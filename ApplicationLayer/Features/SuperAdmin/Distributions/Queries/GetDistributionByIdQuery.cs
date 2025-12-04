using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Queries
{
    public class GetDistributionByIdQuery : IRequest<DistributionDto>
    {
        public int Id { get; set; }
    }
}

