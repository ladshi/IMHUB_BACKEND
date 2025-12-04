using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateFields.Commands
{
    public class UpdateTemplateFieldCommand : IRequest<TemplateFieldDto>
    {
        public int Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public IMHub.Domain.Enums.FieldType FieldType { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsLocked { get; set; }
        public string ValidationRulesJson { get; set; } = "{}";
    }
}

