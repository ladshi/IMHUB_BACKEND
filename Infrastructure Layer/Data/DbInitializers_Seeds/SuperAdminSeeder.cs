using IMHub.Domain.Entities;
using IMHub.Infrastructure.Data;
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

        public SuperAdminSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            // ஏற்கனவே SuperAdmin இருந்தால் ஒன்றும் செய்யாதே
            if (await _context.PlatformAdmins.AnyAsync()) return;

            var superAdmin = new PlatformAdmin
            {
                Name = "System Administrator",
                Email = "admin@imhub.com",
                // குறிப்பு: நீங்கள் சொன்னது போல், இது "Default Password" மட்டும் தான்.
                // Login செய்த பிறகு இதை அவர்கள் மாற்றிக்கொள்ளலாம்.
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                IsDeleted = false
            };

            await _context.PlatformAdmins.AddAsync(superAdmin);
            await _context.SaveChangesAsync();
        }
    }
}
