namespace IMHub.ApplicationLayer.Features.Organizations.Contents
{
    public class ContentDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int TemplateVersionId { get; set; }
        public int? CsvUploadId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string GeneratedPdfUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

