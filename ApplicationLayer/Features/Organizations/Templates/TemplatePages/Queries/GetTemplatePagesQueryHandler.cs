using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplatePages.Queries
{
    public class GetTemplatePagesQueryHandler : IRequestHandler<GetTemplatePagesQuery, List<TemplatePageDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetTemplatePagesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<List<TemplatePageDto>> Handle(GetTemplatePagesQuery request, CancellationToken cancellationToken)
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

            var pages = await _unitOfWork.TemplatePageRepository
                .GetByTemplateVersionIdAsync(request.TemplateVersionId, cancellationToken);

            return pages
                .Select(p => new TemplatePageDto
                {
                    Id = p.Id,
                    TemplateVersionId = p.TemplateVersionId,
                    PageNumber = p.PageNumber,
                    Width = p.Width,
                    Height = p.Height,
                    BackgroundImageUrl = p.BackgroundImageUrl,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToList();
        }
    }
}

