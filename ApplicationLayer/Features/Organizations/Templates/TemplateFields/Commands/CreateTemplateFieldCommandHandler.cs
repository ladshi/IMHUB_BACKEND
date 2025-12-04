using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateFields.Commands
{
    public class CreateTemplateFieldCommandHandler : IRequestHandler<CreateTemplateFieldCommand, TemplateFieldDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateTemplateFieldCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<TemplateFieldDto> Handle(CreateTemplateFieldCommand request, CancellationToken cancellationToken)
        {
            // Validate user is Organization Admin
            if (_currentUserService.Role != "OrgAdmin")
            {
                throw new UnauthorizedAccessException("Only Organization Admins can create template fields.");
            }

            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Validate template page exists and belongs to user's organization
            var templatePage = await _unitOfWork.TemplatePageRepository.GetByIdAsync(request.TemplatePageId);
            if (templatePage == null)
            {
                throw new KeyNotFoundException($"Template page with ID {request.TemplatePageId} not found.");
            }

            var templateVersion = await _unitOfWork.TemplateVersionRepository.GetByIdAsync(templatePage.TemplateVersionId);
            if (templateVersion == null)
            {
                throw new KeyNotFoundException($"Template version not found.");
            }

            var template = await _unitOfWork.TemplateRepository.GetByIdAsync(templateVersion.TemplateId);
            if (template == null || template.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access template from another organization.");
            }

            // Check if field name already exists for this page
            var existingField = await _unitOfWork.TemplateFieldRepository
                .GetByTemplatePageIdAndFieldNameAsync(request.TemplatePageId, request.FieldName, cancellationToken);
            
            if (existingField != null)
            {
                throw new InvalidOperationException(
                    $"Field '{request.FieldName}' already exists for this template page.");
            }

            var templateField = new TemplateField
            {
                TemplatePageId = request.TemplatePageId,
                FieldName = request.FieldName,
                FieldType = request.FieldType,
                X = request.X,
                Y = request.Y,
                Width = request.Width,
                Height = request.Height,
                IsLocked = request.IsLocked,
                ValidationRulesJson = request.ValidationRulesJson
            };

            await _unitOfWork.TemplateFieldRepository.AddAsync(templateField);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new TemplateFieldDto
            {
                Id = templateField.Id,
                TemplatePageId = templateField.TemplatePageId,
                FieldName = templateField.FieldName,
                FieldType = templateField.FieldType.ToString(),
                X = templateField.X,
                Y = templateField.Y,
                Width = templateField.Width,
                Height = templateField.Height,
                IsLocked = templateField.IsLocked,
                ValidationRulesJson = templateField.ValidationRulesJson,
                CreatedAt = templateField.CreatedAt,
                UpdatedAt = templateField.UpdatedAt
            };
        }
    }
}

