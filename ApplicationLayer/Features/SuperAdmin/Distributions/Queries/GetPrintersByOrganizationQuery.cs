using IMHub.ApplicationLayer.Features.SuperAdmin.Printers;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Queries
{
    public class GetPrintersByOrganizationQuery : IRequest<List<PrinterDto>>
    {
        public int OrganizationId { get; set; }
    }
}

