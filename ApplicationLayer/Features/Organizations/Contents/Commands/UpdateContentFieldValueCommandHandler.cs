using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Contents.Commands
{
    public class UpdateContentFieldValueCommandHandler : IRequestHandler<UpdateContentFieldValueCommand, ContentFieldValueDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateContentFieldValueCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ContentFieldValueDto> Handle(UpdateContentFieldValueCommand request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Validate content exists and belongs to user's organization
            var content = await _unitOfWork.ContentRepository.GetByIdAsync(request.ContentId);
            if (content == null)
            {
                throw new KeyNotFoundException($"Content with ID {request.ContentId} not found.");
            }

            if (content.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access content from another organization.");
            }

            // Validate template field exists
            var templateField = await _unitOfWork.TemplateFieldRepository.GetByIdAsync(request.TemplateFieldId);
            if (templateField == null)
            {
                throw new KeyNotFoundException($"Template field with ID {request.TemplateFieldId} not found.");
            }

            // Check if field is locked (Employee cannot edit locked fields)
            if (templateField.IsLocked && _currentUserService.Role == "Employee")
            {
                throw new UnauthorizedAccessException(
                    $"Field '{templateField.FieldName}' is locked and cannot be edited by employees.");
            }

            // Get or create ContentFieldValue
            var fieldValue = await _unitOfWork.ContentFieldValueRepository
                .GetByContentIdAndTemplateFieldIdAsync(request.ContentId, request.TemplateFieldId, cancellationToken);

            if (fieldValue == null)
            {
                // Create new field value
                fieldValue = new ContentFieldValue
                {
                    ContentId = request.ContentId,
                    TemplateFieldId = request.TemplateFieldId,
                    Value = request.Value
                };
                await _unitOfWork.ContentFieldValueRepository.AddAsync(fieldValue);
            }
            else
            {
                // Update existing field value
                fieldValue.Value = request.Value;
                await _unitOfWork.ContentFieldValueRepository.UpdateAsync(fieldValue);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ContentFieldValueDto
            {
                Id = fieldValue.Id,
                ContentId = fieldValue.ContentId,
                TemplateFieldId = fieldValue.TemplateFieldId,
                TemplateFieldName = templateField.FieldName,
                IsLocked = templateField.IsLocked,
                Value = fieldValue.Value
            };
        }
    }
}

