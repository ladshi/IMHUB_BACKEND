namespace IMHub.ApplicationLayer.Features.SuperAdmin.Printers
{
    public class PrinterDto
    {
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool SupportsColor { get; set; }
        public bool SupportsDuplex { get; set; }
        public string ApiKey { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

