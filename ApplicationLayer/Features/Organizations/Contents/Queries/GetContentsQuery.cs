using IMHub.ApplicationLayer.Common.Models;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Contents.Queries
{
    public class GetContentsQuery : IRequest<PagedResult<ContentDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? TemplateVersionId { get; set; }
        public string? Status { get; set; }
    }
}

