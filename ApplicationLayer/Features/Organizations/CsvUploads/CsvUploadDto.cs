namespace IMHub.ApplicationLayer.Features.Organizations.CsvUploads
{
    public class CsvUploadDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public string MappingJson { get; set; } = "{}";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

