using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateFields.Queries
{
    public class GetTemplateFieldsQueryHandler : IRequestHandler<GetTemplateFieldsQuery, List<TemplateFieldDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetTemplateFieldsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<List<TemplateFieldDto>> Handle(GetTemplateFieldsQuery request, CancellationToken cancellationToken)
        {
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

            var fields = await _unitOfWork.TemplateFieldRepository
                .GetByTemplatePageIdAsync(request.TemplatePageId, cancellationToken);

            // Apply IsLocked filter if provided
            if (request.IsLocked.HasValue)
            {
                fields = fields.Where(f => f.IsLocked == request.IsLocked.Value).ToList();
            }

            return fields
                .Select(f => new TemplateFieldDto
                {
                    Id = f.Id,
                    TemplatePageId = f.TemplatePageId,
                    FieldName = f.FieldName,
                    FieldType = f.FieldType.ToString(),
                    X = f.X,
                    Y = f.Y,
                    Width = f.Width,
                    Height = f.Height,
                    IsLocked = f.IsLocked,
                    ValidationRulesJson = f.ValidationRulesJson,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                })
                .ToList();
        }
    }
}

