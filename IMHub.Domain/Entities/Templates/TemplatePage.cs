using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities
{
    // 8. TemplatePage (Structure)
    public class TemplatePage : BaseEntity
    {
        [ForeignKey("TemplateVersion")]
        public int TemplateVersionId { get; set; }
        public TemplateVersion TemplateVersion { get; set; } = null!;

        public int PageNumber { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string BackgroundImageUrl { get; set; } = string.Empty;

        public ICollection<TemplateField> Fields { get; set; } = new List<TemplateField>();
    }
}