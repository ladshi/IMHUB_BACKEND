using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Commands
{
    public class DeletePrinterCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}

