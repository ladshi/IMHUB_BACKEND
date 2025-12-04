using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.Commands
{
    public class UpdateTemplateCommand : IRequest<TemplateDto>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public IMHub.Domain.Enums.TemplateStatus Status { get; set; }
        public string MetadataJson { get; set; } = "{}";
    }
}

