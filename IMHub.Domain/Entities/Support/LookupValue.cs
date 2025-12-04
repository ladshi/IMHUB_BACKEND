using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities.Support
{
    // 22. LookupValue (Dropdowns)
    public class LookupValue : BaseEntity
    {
        public int? OrganizationId { get; set; } // Null = System Global
        public string Category { get; set; } = string.Empty; // "Country", "Department"
        public string Value { get; set; } = string.Empty;
    }
}
