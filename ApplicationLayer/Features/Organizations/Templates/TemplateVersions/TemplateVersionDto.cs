namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateVersions
{
    public class TemplateVersionDto
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int VersionNumber { get; set; }
        public string PdfUrl { get; set; } = string.Empty;
        public string DesignJson { get; set; } = "{}";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

