using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities.Support
{
    // 23. Tag (Categorization)
    public class Tag : BaseEntity
    {
        public int OrganizationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ColorHex { get; set; } = "#FFFFFF";
    }
}
