using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Contents.Commands
{
    public class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, ContentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateContentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ContentDto> Handle(CreateContentCommand request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Validate template version exists and belongs to user's organization
            var templateVersion = await _unitOfWork.TemplateVersionRepository.GetByIdAsync(request.TemplateVersionId);
            if (templateVersion == null)
            {
                throw new KeyNotFoundException($"Template version with ID {request.TemplateVersionId} not found.");
            }

            var template = await _unitOfWork.TemplateRepository.GetByIdAsync(templateVersion.TemplateId);
            if (template == null || template.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access template from another organization.");
            }

            // Create Content instance
            var content = new Content
            {
                OrganizationId = organizationId,
                TemplateVersionId = request.TemplateVersionId,
                CsvUploadId = null, // Manual entry
                Name = request.Name,
                Status = "Draft"
            };

            await _unitOfWork.ContentRepository.AddAsync(content);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Create ContentFieldValue for each field
            foreach (var fieldValue in request.FieldValues)
            {
                var templateField = await _unitOfWork.TemplateFieldRepository.GetByIdAsync(fieldValue.Key);
                if (templateField == null)
                {
                    continue; // Skip invalid field IDs
                }

                // Validate field belongs to template version's pages
                var page = await _unitOfWork.TemplatePageRepository.GetByIdAsync(templateField.TemplatePageId);
                if (page == null || page.TemplateVersionId != request.TemplateVersionId)
                {
                    continue; // Skip invalid field
                }

                // Check if field is locked (Employee cannot set locked fields)
                if (templateField.IsLocked && _currentUserService.Role == "Employee")
                {
                    throw new UnauthorizedAccessException(
                        $"Field '{templateField.FieldName}' is locked and cannot be edited by employees.");
                }

                var contentFieldValue = new ContentFieldValue
                {
                    ContentId = content.Id,
                    TemplateFieldId = fieldValue.Key,
                    Value = fieldValue.Value
                };

                await _unitOfWork.ContentFieldValueRepository.AddAsync(contentFieldValue);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ContentDto
            {
                Id = content.Id,
                OrganizationId = content.OrganizationId,
                TemplateVersionId = content.TemplateVersionId,
                CsvUploadId = content.CsvUploadId,
                Name = content.Name,
                Status = content.Status,
                GeneratedPdfUrl = content.GeneratedPdfUrl,
                CreatedAt = content.CreatedAt,
                UpdatedAt = content.UpdatedAt
            };
        }
    }
}

