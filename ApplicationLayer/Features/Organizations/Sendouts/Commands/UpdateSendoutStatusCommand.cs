using IMHub.Domain.Enums;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Commands
{
    public class UpdateSendoutStatusCommand : IRequest<SendoutDto>
    {
        public int SendoutId { get; set; }
        public SendoutStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}

