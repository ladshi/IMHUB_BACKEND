using IMHub.ApplicationLayer.Features.SuperAdmin.Organizations;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Queries
{
    public class GetOrganizationsByPrinterQuery : IRequest<List<OrganizationDto>>
    {
        public int PrinterId { get; set; }
    }
}

