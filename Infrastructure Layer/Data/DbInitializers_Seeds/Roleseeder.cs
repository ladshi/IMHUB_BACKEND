using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
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

        public RoleSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
           
            if (await _context.Roles.AnyAsync()) return;

            var roles = new List<Role>
            {
                new Role { Name = "SuperAdmin", Description = "Platform Owner", IsSystemRole = true },
                new Role { Name = "OrgAdmin", Description = "Organization Administrator", IsSystemRole = true },
                new Role { Name = "Manager", Description = "Organization Manager", IsSystemRole = true },
                new Role { Name = "Employee", Description = "Standard User", IsSystemRole = true },
                new Role { Name = "PrinterOperator", Description = "Printing Press Staff", IsSystemRole = true }
            };

            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
        }
    }
}
