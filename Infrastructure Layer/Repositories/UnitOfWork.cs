using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Infrastructure.Data;
using IMHub.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository UserRepository { get; }
        public IOrganizationRepository OrganizationRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IPlatformAdminRepository PlatformAdminRepository { get; }
        public IUserRoleRepository UserRoleRepository { get; }
        public IPrinterRepository PrinterRepository { get; }
        public IDistributionRepository DistributionRepository { get; }
        public ITemplateRepository TemplateRepository { get; }
        public ITemplateVersionRepository TemplateVersionRepository { get; }
        public ITemplatePageRepository TemplatePageRepository { get; }
        public ITemplateFieldRepository TemplateFieldRepository { get; }
        public ICsvUploadRepository CsvUploadRepository { get; }
        public IContentRepository ContentRepository { get; }
        public IContentFieldValueRepository ContentFieldValueRepository { get; }
        public ISendoutRepository SendoutRepository { get; }
        public ISendoutStatusHistoryRepository SendoutStatusHistoryRepository { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            UserRepository = new UserRepository(_context);
            OrganizationRepository = new OrganizationRepository(_context);
            RoleRepository = new RoleRepository(_context);
            PlatformAdminRepository = new PlatformAdminRepository(_context);
            UserRoleRepository = new UserRoleRepository(_context);
            PrinterRepository = new PrinterRepository(_context);
            DistributionRepository = new DistributionRepository(_context);
            TemplateRepository = new TemplateRepository(_context);
            TemplateVersionRepository = new TemplateVersionRepository(_context);
            TemplatePageRepository = new TemplatePageRepository(_context);
            TemplateFieldRepository = new TemplateFieldRepository(_context);
            CsvUploadRepository = new CsvUploadRepository(_context);
            ContentRepository = new ContentRepository(_context);
            ContentFieldValueRepository = new ContentFieldValueRepository(_context);
            SendoutRepository = new SendoutRepository(_context);
            SendoutStatusHistoryRepository = new SendoutStatusHistoryRepository(_context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
