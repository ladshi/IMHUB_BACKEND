using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.Infrastruture;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using MediatR;
using System.Text;

namespace IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Commands
{
    public class UploadCsvCommandHandler : IRequestHandler<UploadCsvCommand, CsvUploadDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorageService _fileStorageService;

        public UploadCsvCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileStorageService = fileStorageService;
        }

        public async Task<CsvUploadDto> Handle(UploadCsvCommand request, CancellationToken cancellationToken)
        {
            // Validate user is Organization Admin or Employee
            if (_currentUserService.Role != "OrgAdmin" && _currentUserService.Role != "Employee")
            {
                throw new UnauthorizedAccessException("Only Organization Admins and Employees can upload CSV files.");
            }

            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Validate template exists and belongs to user's organization
            var template = await _unitOfWork.TemplateRepository.GetByIdAsync(request.TemplateId);
            if (template == null)
            {
                throw new KeyNotFoundException($"Template with ID {request.TemplateId} not found.");
            }

            if (template.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access template from another organization.");
            }

            // Validate file
            if (request.File == null || request.File.Length == 0)
            {
                throw new InvalidOperationException("CSV file is required.");
            }

            if (!request.File.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Only CSV files are allowed.");
            }

            // Save file to storage
            var fileUrl = await _fileStorageService.SaveFileAsync(request.File, "csv-uploads");

            // Parse CSV to count rows
            int totalRows = 0;
            using (var reader = new StreamReader(request.File.OpenReadStream(), Encoding.UTF8))
            {
                string? line;
                bool isFirstLine = true;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!isFirstLine && !string.IsNullOrWhiteSpace(line))
                    {
                        totalRows++;
                    }
                    isFirstLine = false;
                }
            }

            var csvUpload = new CsvUpload
            {
                OrganizationId = organizationId,
                TemplateId = request.TemplateId,
                FileName = request.File.FileName,
                FileUrl = fileUrl,
                TotalRows = totalRows,
                MappingJson = "{}"
            };

            await _unitOfWork.CsvUploadRepository.AddAsync(csvUpload);
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

