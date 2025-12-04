namespace IMHub.ApplicationLayer.Features.Organizations.Contents
{
    public class ContentWithFieldsDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int TemplateVersionId { get; set; }
        public int? CsvUploadId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string GeneratedPdfUrl { get; set; } = string.Empty;
        public List<ContentFieldValueDto> FieldValues { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

