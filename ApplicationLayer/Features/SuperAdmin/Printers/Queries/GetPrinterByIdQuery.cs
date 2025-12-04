using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Queries
{
    public class GetPrinterByIdQuery : IRequest<PrinterDto>
    {
        public int Id { get; set; }
    }
}

