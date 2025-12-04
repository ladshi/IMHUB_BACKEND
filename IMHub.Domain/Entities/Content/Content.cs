using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities
{
    // 11. Content (The Instance)
    public class Content : BaseEntity
    {
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }

        [ForeignKey("TemplateVersion")]
        public int TemplateVersionId { get; set; }
        public TemplateVersion TemplateVersion { get; set; } = null!;

        public int? CsvUploadId { get; set; } // Nullable if manual entry

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty; // "Invoice #101"
        public string Status { get; set; } = "Draft";

        public string GeneratedPdfUrl { get; set; } = string.Empty; // Final Proof

        public ICollection<ContentFieldValue> FieldValues { get; set; } = new List<ContentFieldValue>();
    }
}