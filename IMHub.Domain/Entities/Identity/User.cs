using IMHub.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMHub.Domain.Entities
{
    // 3. User (Employee/OrgAdmin)
    public class User : BaseEntity
    {
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Username { get; set; }

        public bool IsActive { get; set; } = true;

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // Navigations
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // New Navigation Property
        public UserProfile? Profile { get; set; } // Link to Profile

        // New Flag: If TRUE, user MUST change password immediately
        public bool IsPasswordChangeRequired { get; set; } = false;
    }
}
