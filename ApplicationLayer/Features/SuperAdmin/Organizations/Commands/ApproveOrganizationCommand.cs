using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands
{
    public class ApproveOrganizationCommand : IRequest<OrganizationDto>
    {
        public int Id { get; set; }
    }
}

