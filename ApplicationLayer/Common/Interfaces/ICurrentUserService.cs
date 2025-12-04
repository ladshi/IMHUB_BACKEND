namespace IMHub.ApplicationLayer.Common.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        int? OrganizationId { get; }
        string? Role { get; }
    }
}

