using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands
{
    public class DeleteOrganizationCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}

