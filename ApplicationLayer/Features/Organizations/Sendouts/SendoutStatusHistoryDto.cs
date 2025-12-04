using IMHub.Domain.Enums;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts
{
    public class SendoutStatusHistoryDto
    {
        public int Id { get; set; }
        public int SendoutId { get; set; }
        public string JobReference { get; set; } = string.Empty;
        public SendoutStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public int UpdatedByUserId { get; set; }
        public string UpdatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

