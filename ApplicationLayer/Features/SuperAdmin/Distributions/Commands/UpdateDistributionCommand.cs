using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Commands
{
    public class UpdateDistributionCommand : IRequest<DistributionDto>
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}

