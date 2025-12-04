using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateFields.Queries
{
    public class GetTemplateFieldsQuery : IRequest<List<TemplateFieldDto>>
    {
        public int TemplatePageId { get; set; }
        public bool? IsLocked { get; set; }
    }
}

