using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<UserRole>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}

