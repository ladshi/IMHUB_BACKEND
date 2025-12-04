using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Commands
{
    public class CreateDistributionCommand : IRequest<DistributionDto>
    {
        public int OrganizationId { get; set; }
        public int PrinterId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

