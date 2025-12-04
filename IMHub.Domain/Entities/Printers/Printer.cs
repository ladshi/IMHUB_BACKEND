using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities
{
    // 13. Printer (The Hardware)
    public class Printer : BaseEntity
    {
        // NULL = Global, Value = Private
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool SupportsColor { get; set; } = true;
        public bool SupportsDuplex { get; set; } = false;

        public string ApiKey { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}