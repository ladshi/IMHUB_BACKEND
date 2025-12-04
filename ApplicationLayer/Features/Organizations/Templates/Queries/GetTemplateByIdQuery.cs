using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.Queries
{
    public class GetTemplateByIdQuery : IRequest<TemplateDto>
    {
        public int Id { get; set; }
    }
}

