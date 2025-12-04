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
    // 6. Template (The Header)
    public class Template : BaseEntity
    {
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        public string ThumbnailUrl { get; set; } = string.Empty;
        public TemplateStatus Status { get; set; } = TemplateStatus.Draft;
        public string MetadataJson { get; set; } = "{}";

        public ICollection<TemplateVersion> Versions { get; set; } = new List<TemplateVersion>();
    }
}
