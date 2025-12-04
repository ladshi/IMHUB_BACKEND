using IMHub.Domain.Enums;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts
{
    public class SendoutDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public int ContentId { get; set; }
        public string ContentName { get; set; } = string.Empty;
        public int PrinterId { get; set; }
        public string PrinterName { get; set; } = string.Empty;
        public string JobReference { get; set; } = string.Empty;
        public SendoutStatus CurrentStatus { get; set; }
        public string CurrentStatusName { get; set; } = string.Empty;
        public DateTime TargetDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

