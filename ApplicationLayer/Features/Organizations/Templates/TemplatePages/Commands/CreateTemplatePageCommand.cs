using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplatePages.Commands
{
    public class CreateTemplatePageCommand : IRequest<TemplatePageDto>
    {
        public int TemplateVersionId { get; set; }
        public int PageNumber { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string BackgroundImageUrl { get; set; } = string.Empty;
    }
}

