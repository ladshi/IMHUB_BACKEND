using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateVersions.Queries
{
    public class GetTemplateVersionsQueryHandler : IRequestHandler<GetTemplateVersionsQuery, List<TemplateVersionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetTemplateVersionsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<List<TemplateVersionDto>> Handle(GetTemplateVersionsQuery request, CancellationToken cancellationToken)
        {
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

            var versions = await _unitOfWork.TemplateVersionRepository
                .GetByTemplateIdAsync(request.TemplateId, cancellationToken);

            return versions
                .OrderBy(v => v.VersionNumber)
                .Select(v => new TemplateVersionDto
                {
                    Id = v.Id,
                    TemplateId = v.TemplateId,
                    VersionNumber = v.VersionNumber,
                    PdfUrl = v.PdfUrl,
                    DesignJson = v.DesignJson,
                    IsActive = v.IsActive,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt
                })
                .ToList();
        }
    }
}

