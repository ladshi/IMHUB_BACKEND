using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands
{
    public class CreateOrganizationCommand : IRequest<OrganizationDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string TenantCode { get; set; } = string.Empty;
        public IMHub.Domain.Enums.PlanType PlanType { get; set; } = IMHub.Domain.Enums.PlanType.Free;
        public string LimitsJson { get; set; } = "{}";
        public bool IsActive { get; set; } = true;
    }
}

