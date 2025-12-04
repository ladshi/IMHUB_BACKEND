using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Queries
{
    public class GetSendoutByIdQuery : IRequest<SendoutWithHistoryDto>
    {
        public int Id { get; set; }
    }
}

