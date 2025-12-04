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
    // 10. CsvUpload (Bulk Source)
    public class CsvUpload : BaseEntity
    {
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }

        [ForeignKey("Template")]
        public int TemplateId { get; set; }

        [Required, MaxLength(250)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FileUrl { get; set; } = string.Empty;

        public int TotalRows { get; set; }
        public string MappingJson { get; set; } = "{}"; // Map CSV Col -> Template Field
    }
}