using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities.Workflow
{
    // 20. AuditLog (Security)
    public class AuditLog : BaseEntity
    {
        public int? OrganizationId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string ChangesJson { get; set; } = "{}";
    }
}
