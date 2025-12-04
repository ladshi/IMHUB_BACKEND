using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Commands
{
    public class DeleteDistributionCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}

