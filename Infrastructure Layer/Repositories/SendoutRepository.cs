using IMHub.ApplicationLayer.Common.Interfaces.Repositories;
using IMHub.Domain.Entities;
using IMHub.Domain.Enums;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IMHub.Infrastructure.Repositories
{
    public class SendoutRepository : GenericRepository<Sendout>, ISendoutRepository
    {
        public SendoutRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Sendout>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default)
        {
            return await _context.Sendouts
                .Include(s => s.Content)
                .Include(s => s.Printer)
                .Include(s => s.Organization)
                .Where(s => s.OrganizationId == organizationId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Sendout>> GetByContentIdAsync(int contentId, CancellationToken cancellationToken = default)
        {
            return await _context.Sendouts
                .Include(s => s.Content)
                .Include(s => s.Printer)
                .Include(s => s.Organization)
                .Where(s => s.ContentId == contentId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Sendout>> GetByPrinterIdAsync(int printerId, CancellationToken cancellationToken = default)
        {
            return await _context.Sendouts
                .Include(s => s.Content)
                .Include(s => s.Printer)
                .Include(s => s.Organization)
                .Where(s => s.PrinterId == printerId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Sendout>> GetByStatusAsync(SendoutStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Sendouts
                .Include(s => s.Content)
                .Include(s => s.Printer)
                .Include(s => s.Organization)
                .Where(s => s.CurrentStatus == status)
                .ToListAsync(cancellationToken);
        }

        public async Task<Sendout?> GetByJobReferenceAsync(string jobReference, CancellationToken cancellationToken = default)
        {
            return await _context.Sendouts
                .Include(s => s.Content)
                .Include(s => s.Printer)
                .Include(s => s.Organization)
                .Include(s => s.History)
                .FirstOrDefaultAsync(s => s.JobReference == jobReference, cancellationToken);
        }
    }
}

