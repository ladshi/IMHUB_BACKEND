using IMHub.ApplicationLayer.Common.Models;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.Queries
{
    public class GetTemplatesQuery : IRequest<PagedResult<TemplateDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
    }
}

