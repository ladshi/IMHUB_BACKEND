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
    // 7. TemplateVersion (Versioning Support)
    public class TemplateVersion : BaseEntity
    {
        [ForeignKey("Template")]
        public int TemplateId { get; set; }
        public Template Template { get; set; } = null!;

        public int VersionNumber { get; set; }

        [Required]
        public string PdfUrl { get; set; } = string.Empty; // Blob URL

        public string DesignJson { get; set; } = "{}"; // Global Config
        public bool IsActive { get; set; } = true;

        public ICollection<TemplatePage> Pages { get; set; } = new List<TemplatePage>();
    }
}