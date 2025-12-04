using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class SendoutStatusHistoryRepository : GenericRepository<SendoutStatusHistory>, ISendoutStatusHistoryRepository
    {
        public SendoutStatusHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<SendoutStatusHistory>> GetBySendoutIdAsync(int sendoutId, CancellationToken cancellationToken = default)
        {
            return await _context.SendoutStatusHistories
                .Include(h => h.Sendout)
                .Where(h => h.SendoutId == sendoutId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<SendoutStatusHistory>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.SendoutStatusHistories
                .Include(h => h.Sendout)
                .Where(h => h.UpdatedByUserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}

