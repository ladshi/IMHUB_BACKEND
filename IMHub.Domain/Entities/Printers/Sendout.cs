using IMHub.Domain.Common;
using IMHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities
{
    // 14. Sendout (The Job)
    public class Sendout : BaseEntity
    {
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;

        [ForeignKey("Content")]
        public int ContentId { get; set; }
        public Content Content { get; set; } = null!;

        [ForeignKey("Printer")]
        public int PrinterId { get; set; }
        public Printer Printer { get; set; } = null!;

        [Required, MaxLength(50)]
        public string JobReference { get; set; } = string.Empty; // Unique ID

        public SendoutStatus CurrentStatus { get; set; } = SendoutStatus.Submitted;
        public DateTime TargetDate { get; set; }

        public ICollection<SendoutStatusHistory> History { get; set; } = new List<SendoutStatusHistory>();
    }
}