using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateVersions.Commands
{
    public class SetActiveVersionCommand : IRequest<TemplateVersionDto>
    {
        public int TemplateId { get; set; }
        public int VersionId { get; set; }
    }
}

