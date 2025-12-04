using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.Commands
{
    public class CreateTemplateCommand : IRequest<TemplateDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public IMHub.Domain.Enums.TemplateStatus Status { get; set; } = IMHub.Domain.Enums.TemplateStatus.Draft;
        public string MetadataJson { get; set; } = "{}";
    }
}

