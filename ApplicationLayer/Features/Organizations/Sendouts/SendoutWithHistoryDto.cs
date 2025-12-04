namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts
{
    public class SendoutWithHistoryDto
    {
        public SendoutDto Sendout { get; set; } = null!;
        public List<SendoutStatusHistoryDto> History { get; set; } = new List<SendoutStatusHistoryDto>();
    }
}

