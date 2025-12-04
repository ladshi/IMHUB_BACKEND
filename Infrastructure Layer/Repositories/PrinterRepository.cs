using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class PrinterRepository : GenericRepository<Printer>, IPrinterRepository
    {
        public PrinterRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Printer?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Printers
                .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        }

        public async Task<IReadOnlyList<Printer>> GetByOrganizationIdAsync(int? organizationId, CancellationToken cancellationToken = default)
        {
            return await _context.Printers
                .Where(p => p.OrganizationId == organizationId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Printer>> GetActivePrintersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Printers
                .Where(p => p.IsActive)
                .ToListAsync(cancellationToken);
        }
    }
}

