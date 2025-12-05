using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Custom query needed for Login (includes Roles, Org, Profile)
        Task<User?> GetUserByEmailOrUsernameAsync(string identifier, CancellationToken cancellationToken = default);
        Task<User?> GetUserWithRolesAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default);
        Task<int> ActivateUsersByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default);
    }
}
