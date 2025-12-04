using IMHub.ApplicationLayer.Common.Models;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Queries
{
    public class GetPrintersQuery : IRequest<PagedResult<PrinterDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int? OrganizationId { get; set; }
        public bool? IsActive { get; set; }
    }
}

