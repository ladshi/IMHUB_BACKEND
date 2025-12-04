using IMHub.Domain.Entities;

namespace IMHub.ApplicationLayer.Common.Interfaces.Repositories
{
    public interface ITemplateRepository : IGenericRepository<Template>
    {
        Task<IReadOnlyList<Template>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default);
        Task<Template?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
    }
}

