namespace IMHub.ApplicationLayer.Features.Organizations.Contents
{
    public class ContentFieldValueDto
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        public int TemplateFieldId { get; set; }
        public string TemplateFieldName { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}

