using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface ISendoutStatusHistoryRepository : IGenericRepository<SendoutStatusHistory>
    {
        Task<IReadOnlyList<SendoutStatusHistory>> GetBySendoutIdAsync(int sendoutId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SendoutStatusHistory>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }
}

