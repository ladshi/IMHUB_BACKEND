using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities
{
    // 4. Role (Lookup Table)
    public class Role : BaseEntity
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty; // "OrgAdmin", "Manager"
        public string Description { get; set; } = string.Empty;
    }
}