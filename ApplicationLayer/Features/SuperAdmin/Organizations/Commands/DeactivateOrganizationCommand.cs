using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands
{
    public class DeactivateOrganizationCommand : IRequest<OrganizationDto>
    {
        public int Id { get; set; }
    }
}

