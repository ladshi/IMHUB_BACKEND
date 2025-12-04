using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateVersions.Queries
{
    public class GetTemplateVersionsQuery : IRequest<List<TemplateVersionDto>>
    {
        public int TemplateId { get; set; }
    }
}

