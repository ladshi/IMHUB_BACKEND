using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.ApplicationLayer.Features.Auth.Commands
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? OrganizationId { get; set; } // Null for SuperAdmin
        public string Token { get; set; } = string.Empty;
    }
}
