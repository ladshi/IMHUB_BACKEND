namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations
{
    public class OrganizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string TenantCode { get; set; } = string.Empty;
        public string PlanType { get; set; } = string.Empty;
        public string LimitsJson { get; set; } = "{}";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

