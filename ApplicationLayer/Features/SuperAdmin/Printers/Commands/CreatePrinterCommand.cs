using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Commands
{
    public class CreatePrinterCommand : IRequest<PrinterDto>
    {
        public int? OrganizationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool SupportsColor { get; set; } = true;
        public bool SupportsDuplex { get; set; } = false;
        public string ApiKey { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}

