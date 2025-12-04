using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Contents.Queries
{
    public class GetContentByIdQueryHandler : IRequestHandler<GetContentByIdQuery, ContentWithFieldsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetContentByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ContentWithFieldsDto> Handle(GetContentByIdQuery request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            var content = await _unitOfWork.ContentRepository.GetByIdAsync(request.Id);
            if (content == null)
            {
                throw new KeyNotFoundException($"Content with ID {request.Id} not found.");
            }

            // Ensure content belongs to user's organization
            if (content.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access content from another organization.");
            }

            // Get field values
            var fieldValues = await _unitOfWork.ContentFieldValueRepository
                .GetByContentIdAsync(request.Id, cancellationToken);

            // Get template fields for field names
            var fieldValueDtos = new List<ContentFieldValueDto>();
            foreach (var fieldValue in fieldValues)
            {
                var templateField = await _unitOfWork.TemplateFieldRepository.GetByIdAsync(fieldValue.TemplateFieldId);
                if (templateField != null)
                {
                    fieldValueDtos.Add(new ContentFieldValueDto
                    {
                        Id = fieldValue.Id,
                        ContentId = fieldValue.ContentId,
                        TemplateFieldId = fieldValue.TemplateFieldId,
                        TemplateFieldName = templateField.FieldName,
                        IsLocked = templateField.IsLocked,
                        Value = fieldValue.Value
                    });
                }
            }

            return new ContentWithFieldsDto
            {
                Id = content.Id,
                OrganizationId = content.OrganizationId,
                TemplateVersionId = content.TemplateVersionId,
                CsvUploadId = content.CsvUploadId,
                Name = content.Name,
                Status = content.Status,
                GeneratedPdfUrl = content.GeneratedPdfUrl,
                FieldValues = fieldValueDtos,
                CreatedAt = content.CreatedAt,
                UpdatedAt = content.UpdatedAt
            };
        }
    }
}

