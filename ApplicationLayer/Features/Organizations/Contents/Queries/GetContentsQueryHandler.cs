using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Common.Models;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Contents.Queries
{
    public class GetContentsQueryHandler : IRequestHandler<GetContentsQuery, PagedResult<ContentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetContentsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<ContentDto>> Handle(GetContentsQuery request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Get contents for user's organization only
            var contents = await _unitOfWork.ContentRepository
                .GetByOrganizationIdAsync(organizationId, cancellationToken);

            // Apply TemplateVersionId filter if provided
            if (request.TemplateVersionId.HasValue)
            {
                contents = contents.Where(c => c.TemplateVersionId == request.TemplateVersionId.Value).ToList();
            }

            // Apply Status filter if provided
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                contents = contents.Where(c => c.Status == request.Status).ToList();
            }

            var totalCount = contents.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Apply pagination
            var pagedContents = contents
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new ContentDto
                {
                    Id = c.Id,
                    OrganizationId = c.OrganizationId,
                    TemplateVersionId = c.TemplateVersionId,
                    CsvUploadId = c.CsvUploadId,
                    Name = c.Name,
                    Status = c.Status,
                    GeneratedPdfUrl = c.GeneratedPdfUrl,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToList();

            return new PagedResult<ContentDto>
            {
                Items = pagedContents,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}

