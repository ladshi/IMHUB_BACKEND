using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Features.SuperAdmin.Organizations;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Queries
{
    public class GetOrganizationsByPrinterQueryHandler : IRequestHandler<GetOrganizationsByPrinterQuery, List<OrganizationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOrganizationsByPrinterQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<OrganizationDto>> Handle(GetOrganizationsByPrinterQuery request, CancellationToken cancellationToken)
        {
            var distributions = await _unitOfWork.DistributionRepository.GetByPrinterIdAsync(request.PrinterId, cancellationToken);
            
            var organizationIds = distributions.Select(d => d.OrganizationId).ToList();
            var allOrganizations = await _unitOfWork.OrganizationRepository.GetAllAsync();
            
            var organizations = allOrganizations
                .Where(o => organizationIds.Contains(o.Id))
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

            return organizations;
        }
    }
}

