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
    public class RoleSeeder : ICustomSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleSeeder> _logger;

        public RoleSeeder(ApplicationDbContext context, ILogger<RoleSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            // Check if roles already exist
            var existingRolesCount = await _context.Roles.CountAsync();
            if (existingRolesCount > 0)
            {
                _logger.LogInformation("Roles already exist ({Count} roles found). Skipping role seeding.", existingRolesCount);
                return;
            }

            _logger.LogInformation("Creating default roles...");

            var roles = new List<Role>
            {
                new Role { Name = "SuperAdmin", Description = "Platform Owner" },
                new Role { Name = "OrgAdmin", Description = "Organization Administrator" },
                new Role { Name = "Manager", Description = "Organization Manager" },
                new Role { Name = "Employee", Description = "Standard User" },
                new Role { Name = "PrinterOperator", Description = "Printing Press Staff" }
            };

            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully created {Count} roles: {Roles}", 
                roles.Count, 
                string.Join(", ", roles.Select(r => r.Name)));
        }
    }
}
