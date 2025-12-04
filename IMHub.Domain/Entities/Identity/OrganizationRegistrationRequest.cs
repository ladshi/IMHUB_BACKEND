using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities.Identity
{
    public class OrganizationRegistrationRequest : BaseEntity
    {
        public string OrgName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminName { get; set; } = string.Empty;
        public string AdminPhone { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string? RejectionReason { get; set; }
    }
}
