using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class PlatformAdminRepository : GenericRepository<PlatformAdmin>, IPlatformAdminRepository
    {
        public PlatformAdminRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PlatformAdmin?> GetByEmailOrNameAsync(string identifier, CancellationToken cancellationToken = default)
        {
            return await _context.PlatformAdmins
                .FirstOrDefaultAsync(x => x.Email == identifier || x.Name == identifier, cancellationToken);
        }
    }
}

