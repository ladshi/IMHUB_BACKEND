using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
        }
    }
}

