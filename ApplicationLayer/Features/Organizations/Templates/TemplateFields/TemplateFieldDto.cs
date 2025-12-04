namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateFields
{
    public class TemplateFieldDto
    {
        public int Id { get; set; }
        public int TemplatePageId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsLocked { get; set; }
        public string ValidationRulesJson { get; set; } = "{}";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

