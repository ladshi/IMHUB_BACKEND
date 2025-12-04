using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities.Workflow
{
    // 21. NotificationLog (Email History)
    public class NotificationLog : BaseEntity
    {
        public int? OrganizationId { get; set; }
        public string RecipientEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Status { get; set; } = "Sent";
    }
}
