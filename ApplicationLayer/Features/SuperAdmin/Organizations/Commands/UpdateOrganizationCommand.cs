using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands
{
    public class UpdateOrganizationCommand : IRequest<OrganizationDto>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public IMHub.Domain.Enums.PlanType PlanType { get; set; }
        public string LimitsJson { get; set; } = "{}";
        public bool IsActive { get; set; }
    }
}

