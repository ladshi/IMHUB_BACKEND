using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Queries
{
    public class GetOrganizationsQueryHandler : IRequestHandler<GetOrganizationsQuery, PagedResult<OrganizationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOrganizationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
        {
            var organizations = await _unitOfWork.OrganizationRepository.GetAllAsync();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                organizations = organizations.Where(o =>
                    o.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    o.Domain.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    o.TenantCode.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Apply IsActive filter if provided
            if (request.IsActive.HasValue)
            {
                organizations = organizations.Where(o => o.IsActive == request.IsActive.Value).ToList();
            }

            var totalCount = organizations.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Apply pagination
            var pagedOrganizations = organizations
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OrganizationDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Domain = o.Domain,
                    TenantCode = o.TenantCode,
                    PlanType = o.PlanType.ToString(),
                    LimitsJson = o.LimitsJson,
                    IsActive = o.IsActive,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                })
                .ToList();

            return new PagedResult<OrganizationDto>
            {
                Items = pagedOrganizations,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}

