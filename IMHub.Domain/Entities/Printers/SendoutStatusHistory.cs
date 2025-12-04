using IMHub.Domain.Common;
using IMHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities
{
    // 15. SendoutStatusHistory (Audit Trail)
    public class SendoutStatusHistory : BaseEntity
    {
        [ForeignKey("Sendout")]
        public int SendoutId { get; set; }
        public Sendout Sendout { get; set; } = null!;

        public SendoutStatus Status { get; set; }
        public string Notes { get; set; } = string.Empty;

        public int UpdatedByUserId { get; set; } // Who changed it?
    }
}
