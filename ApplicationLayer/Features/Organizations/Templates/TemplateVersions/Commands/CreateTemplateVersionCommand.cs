using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateVersions.Commands
{
    public class CreateTemplateVersionCommand : IRequest<TemplateVersionDto>
    {
        public int TemplateId { get; set; }
        public int VersionNumber { get; set; }
        public string PdfUrl { get; set; } = string.Empty;
        public string DesignJson { get; set; } = "{}";
        public bool IsActive { get; set; } = true;
    }
}

