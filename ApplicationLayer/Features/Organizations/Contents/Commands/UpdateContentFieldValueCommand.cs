using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Contents.Commands
{
    public class UpdateContentFieldValueCommand : IRequest<ContentFieldValueDto>
    {
        public int ContentId { get; set; }
        public int TemplateFieldId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}

