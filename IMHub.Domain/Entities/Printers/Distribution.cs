using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities
{
    // 16. Distribution (Routing Logic - Optional Advanced Feature)
    public class Distribution : BaseEntity
    {
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        [ForeignKey("Printer")]
        public int PrinterId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}