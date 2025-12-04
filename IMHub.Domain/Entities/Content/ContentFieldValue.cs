using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities
{
    // 12. ContentFieldValue (The Data - 3NF Compliant)
    public class ContentFieldValue : BaseEntity
    {
        [ForeignKey("Content")]
        public int ContentId { get; set; }
        public Content Content { get; set; } = null!;

        [ForeignKey("TemplateField")]
        public int TemplateFieldId { get; set; }
        public TemplateField TemplateField { get; set; } = null!;

        public string Value { get; set; } = string.Empty; // "John Doe"
    }
}