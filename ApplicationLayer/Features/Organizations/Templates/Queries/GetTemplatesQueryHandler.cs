using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Common.Models;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.Queries
{
    public class GetTemplatesQueryHandler : IRequestHandler<GetTemplatesQuery, PagedResult<TemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetTemplatesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<TemplateDto>> Handle(GetTemplatesQuery request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Get templates for user's organization only (multi-tenant isolation)
            var templates = await _unitOfWork.TemplateRepository.GetByOrganizationIdAsync(organizationId, cancellationToken);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                templates = templates.Where(t =>
                    t.Title.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.Slug.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Apply status filter if provided
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<IMHub.Domain.Enums.TemplateStatus>(request.Status, out var status))
                {
                    templates = templates.Where(t => t.Status == status).ToList();
                }
            }

            var totalCount = templates.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Apply pagination
            var pagedTemplates = templates
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TemplateDto
                {
                    Id = t.Id,
                    OrganizationId = t.OrganizationId,
                    Title = t.Title,
                    Slug = t.Slug,
                    ThumbnailUrl = t.ThumbnailUrl,
                    Status = t.Status.ToString(),
                    MetadataJson = t.MetadataJson,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToList();

            return new PagedResult<TemplateDto>
            {
                Items = pagedTemplates,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}

