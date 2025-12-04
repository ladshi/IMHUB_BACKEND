using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Queries
{
    public class GetOrganizationByIdQuery : IRequest<OrganizationDto>
    {
        public int Id { get; set; }
    }
}

