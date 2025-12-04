using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Commands
{
    public class CreateSendoutCommand : IRequest<SendoutDto>
    {
        public int ContentId { get; set; }
        public int PrinterId { get; set; }
        public DateTime TargetDate { get; set; }
    }
}

