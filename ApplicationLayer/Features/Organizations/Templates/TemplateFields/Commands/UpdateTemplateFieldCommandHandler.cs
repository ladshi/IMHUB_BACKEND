using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateFields.Commands
{
    public class UpdateTemplateFieldCommandHandler : IRequestHandler<UpdateTemplateFieldCommand, TemplateFieldDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateTemplateFieldCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<TemplateFieldDto> Handle(UpdateTemplateFieldCommand request, CancellationToken cancellationToken)
        {
            // Validate user is Organization Admin
            if (_currentUserService.Role != "OrgAdmin")
            {
                throw new UnauthorizedAccessException("Only Organization Admins can update template fields.");
            }

            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            var templateField = await _unitOfWork.TemplateFieldRepository.GetByIdAsync(request.Id);
            if (templateField == null)
            {
                throw new KeyNotFoundException($"Template field with ID {request.Id} not found.");
            }

            // Validate template page exists and belongs to user's organization
            var templatePage = await _unitOfWork.TemplatePageRepository.GetByIdAsync(templateField.TemplatePageId);
            if (templatePage == null)
            {
                throw new KeyNotFoundException($"Template page not found.");
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

            // Check if field name is being changed and if new name already exists
            if (templateField.FieldName != request.FieldName)
            {
                var existingField = await _unitOfWork.TemplateFieldRepository
                    .GetByTemplatePageIdAndFieldNameAsync(templateField.TemplatePageId, request.FieldName, cancellationToken);
                
                if (existingField != null && existingField.Id != request.Id)
                {
                    throw new InvalidOperationException(
                        $"Field '{request.FieldName}' already exists for this template page.");
                }
            }

            templateField.FieldName = request.FieldName;
            templateField.FieldType = request.FieldType;
            templateField.X = request.X;
            templateField.Y = request.Y;
            templateField.Width = request.Width;
            templateField.Height = request.Height;
            templateField.IsLocked = request.IsLocked;
            templateField.ValidationRulesJson = request.ValidationRulesJson;

            await _unitOfWork.TemplateFieldRepository.UpdateAsync(templateField);
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

