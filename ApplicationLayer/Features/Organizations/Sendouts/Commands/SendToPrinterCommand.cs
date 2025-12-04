using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Commands
{
    public class SendToPrinterCommand : IRequest<SendoutDto>
    {
        public int SendoutId { get; set; }
    }
}

