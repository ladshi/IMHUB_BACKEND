using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.Commands
{
    public class DeleteTemplateCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}

