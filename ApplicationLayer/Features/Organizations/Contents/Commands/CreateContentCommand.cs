using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Contents.Commands
{
    public class CreateContentCommand : IRequest<ContentDto>
    {
        public int TemplateVersionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Dictionary<int, string> FieldValues { get; set; } = new(); // TemplateFieldId -> Value
    }
}

