namespace IMHub.ApplicationLayer.Features.Organizations.Templates
{
    public class TemplateDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string MetadataJson { get; set; } = "{}";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

