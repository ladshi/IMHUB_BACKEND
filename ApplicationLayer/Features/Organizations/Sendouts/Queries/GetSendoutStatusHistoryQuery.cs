using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Queries
{
    public class GetSendoutStatusHistoryQuery : IRequest<List<SendoutStatusHistoryDto>>
    {
        public int SendoutId { get; set; }
    }
}

