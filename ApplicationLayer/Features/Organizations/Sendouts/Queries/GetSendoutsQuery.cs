using IMHub.ApplicationLayer.Common.Models;
using IMHub.Domain.Enums;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Queries
{
    public class GetSendoutsQuery : IRequest<PagedResult<SendoutDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public SendoutStatus? Status { get; set; }
        public int? ContentId { get; set; }
        public int? PrinterId { get; set; }
    }
}

