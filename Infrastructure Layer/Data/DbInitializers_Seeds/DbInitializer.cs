using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMHub.Infrastructure.Data.DbInitializers_Seeds
{
    public class DbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DbInitializer> _logger;
        private readonly IEnumerable<ICustomSeeder> _seeders;

        public DbInitializer(
            ApplicationDbContext context,
            ILogger<DbInitializer> logger,
            IEnumerable<ICustomSeeder> seeders)
        {
            _context = context;
            _logger = logger;
            _seeders = seeders;
        }

        public async Task RunAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }

                foreach (var seeder in _seeders)
                {
                    await seeder.InitializeAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }
    }
}
