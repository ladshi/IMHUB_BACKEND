using IMHub.ApplicationLayer.Common.Models;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Queries
{
    public class GetDistributionsQuery : IRequest<PagedResult<DistributionDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? OrganizationId { get; set; }
        public int? PrinterId { get; set; }
        public bool? IsActive { get; set; }
    }
}

