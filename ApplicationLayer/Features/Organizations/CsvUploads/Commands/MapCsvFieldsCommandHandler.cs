using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;
using System.Text.Json;

namespace IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Commands
{
    public class MapCsvFieldsCommandHandler : IRequestHandler<MapCsvFieldsCommand, CsvUploadDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public MapCsvFieldsCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<CsvUploadDto> Handle(MapCsvFieldsCommand request, CancellationToken cancellationToken)
        {
            // Validate user is Organization Admin or Employee
            if (_currentUserService.Role != "OrgAdmin" && _currentUserService.Role != "Employee")
            {
                throw new UnauthorizedAccessException("Only Organization Admins and Employees can map CSV fields.");
            }

            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            var csvUpload = await _unitOfWork.CsvUploadRepository.GetByIdAsync(request.CsvUploadId);
            if (csvUpload == null)
            {
                throw new KeyNotFoundException($"CSV upload with ID {request.CsvUploadId} not found.");
            }

            // Ensure CSV upload belongs to user's organization
            if (csvUpload.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access CSV upload from another organization.");
            }

            // Validate template exists
            var template = await _unitOfWork.TemplateRepository.GetByIdAsync(csvUpload.TemplateId);
            if (template == null)
            {
                throw new KeyNotFoundException($"Template not found.");
            }

            // Get active template version
            var activeVersion = await _unitOfWork.TemplateVersionRepository
                .GetActiveVersionByTemplateIdAsync(csvUpload.TemplateId, cancellationToken);
            
            if (activeVersion == null)
            {
                throw new InvalidOperationException("No active template version found. Please set an active version first.");
            }

            // Get all pages for the template version
            var pages = await _unitOfWork.TemplatePageRepository
                .GetByTemplateVersionIdAsync(activeVersion.Id, cancellationToken);

            // Validate all template field names exist
            var allFields = new List<IMHub.Domain.Entities.TemplateField>();
            foreach (var page in pages)
            {
                var fields = await _unitOfWork.TemplateFieldRepository
                    .GetByTemplatePageIdAsync(page.Id, cancellationToken);
                allFields.AddRange(fields);
            }

            var templateFieldNames = allFields.Select(f => f.FieldName).ToHashSet();

            foreach (var mapping in request.Mappings)
            {
                if (!templateFieldNames.Contains(mapping.Value))
                {
                    throw new InvalidOperationException(
                        $"Template field '{mapping.Value}' does not exist in the template.");
                }

                // Check if field is locked (warn but allow)
                var field = allFields.FirstOrDefault(f => f.FieldName == mapping.Value);
                if (field != null && field.IsLocked)
                {
                    // Log warning but allow mapping (Admin can still map, but Employee won't be able to edit)
                }
            }

            // Save mapping as JSON
            csvUpload.MappingJson = JsonSerializer.Serialize(request.Mappings);

            await _unitOfWork.CsvUploadRepository.UpdateAsync(csvUpload);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CsvUploadDto
            {
                Id = csvUpload.Id,
                OrganizationId = csvUpload.OrganizationId,
                TemplateId = csvUpload.TemplateId,
                TemplateName = template.Title,
                FileName = csvUpload.FileName,
                FileUrl = csvUpload.FileUrl,
                TotalRows = csvUpload.TotalRows,
                MappingJson = csvUpload.MappingJson,
                CreatedAt = csvUpload.CreatedAt,
                UpdatedAt = csvUpload.UpdatedAt
            };
        }
    }
}

