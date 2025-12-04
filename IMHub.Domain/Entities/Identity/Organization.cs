using IMHub.Domain.Common;
using IMHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace IMHub.Domain.Entities
{
    // 2. Organization (Tenant)
    public class Organization : BaseEntity
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Domain { get; set; } = string.Empty; // Unique Index needed in DB

        [Required, MaxLength(20)]
        public string TenantCode { get; set; } = string.Empty; // Unique Identifier

        public PlanType PlanType { get; set; } = PlanType.Free;

        public string LimitsJson { get; set; } = "{}"; // Scalable limits
        public bool IsActive { get; set; } = true;

        // Navigations
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Printer> Printers { get; set; } = new List<Printer>();
    }
}