using IMHub.ApplicationLayer.Common.Models;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Queries
{
    public class GetCsvUploadsQuery : IRequest<PagedResult<CsvUploadDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? TemplateId { get; set; }
    }
}

