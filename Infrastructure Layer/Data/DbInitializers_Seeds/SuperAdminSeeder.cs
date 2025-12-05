using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Infrastructure.Data.DbInitializers_Seeds
{
    public class SuperAdminSeeder : ICustomSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SuperAdminSeeder> _logger;

        public SuperAdminSeeder(ApplicationDbContext context, ILogger<SuperAdminSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            // Check if SuperAdmin already exists
            var existingAdmin = await _context.PlatformAdmins
                .FirstOrDefaultAsync(x => x.Email == "admin@imhub.com");

            if (existingAdmin != null)
            {
                _logger.LogInformation("SuperAdmin already exists: {Email}", existingAdmin.Email);
                return;
            }

            _logger.LogInformation("Creating SuperAdmin user...");
            
            var password = "Admin@123";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            
            _logger.LogInformation("Password hash generated for SuperAdmin");

            var superAdmin = new PlatformAdmin
            {
                Name = "System Administrator",
                Email = "admin@imhub.com",
                PasswordHash = passwordHash,
                IsDeleted = false
            };

            await _context.PlatformAdmins.AddAsync(superAdmin);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("SuperAdmin created successfully: {Email}", superAdmin.Email);
        }
    }
}
