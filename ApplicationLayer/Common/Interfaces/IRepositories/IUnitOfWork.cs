using IMHub.ApplicationLayer.Common.Interfaces.Repositories;

namespace IMHub.ApplicationLayer.Common.Interfaces.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IOrganizationRepository OrganizationRepository { get; }
        IRoleRepository RoleRepository { get; }
        IPlatformAdminRepository PlatformAdminRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        IPrinterRepository PrinterRepository { get; }
        IDistributionRepository DistributionRepository { get; }
        ITemplateRepository TemplateRepository { get; }
        ITemplateVersionRepository TemplateVersionRepository { get; }
        ITemplatePageRepository TemplatePageRepository { get; }
        ITemplateFieldRepository TemplateFieldRepository { get; }
        ICsvUploadRepository CsvUploadRepository { get; }
        IContentRepository ContentRepository { get; }
        IContentFieldValueRepository ContentFieldValueRepository { get; }
        ISendoutRepository SendoutRepository { get; }
        ISendoutStatusHistoryRepository SendoutStatusHistoryRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
