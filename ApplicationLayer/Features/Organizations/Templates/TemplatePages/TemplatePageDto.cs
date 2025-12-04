namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplatePages
{
    public class TemplatePageDto
    {
        public int Id { get; set; }
        public int TemplateVersionId { get; set; }
        public int PageNumber { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string BackgroundImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

