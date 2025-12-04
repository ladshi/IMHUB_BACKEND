namespace IMHub.ApplicationLayer.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(int userId, string email, string role, int? orgId);
    }
}

