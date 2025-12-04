using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplatePages.Queries
{
    public class GetTemplatePagesQuery : IRequest<List<TemplatePageDto>>
    {
        public int TemplateVersionId { get; set; }
    }
}

