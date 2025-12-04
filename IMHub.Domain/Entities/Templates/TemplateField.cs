using IMHub.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IMHub.Domain.Enums;

namespace IMHub.Domain.Entities
{
    // 9. TemplateField (Editable Zones)
    public class TemplateField : BaseEntity
    {
        [ForeignKey("TemplatePage")]
        public int TemplatePageId { get; set; }
        public TemplatePage TemplatePage { get; set; } = null!;

        [Required, MaxLength(100)]
        public string FieldName { get; set; } = string.Empty; // "CustomerName"

        public FieldType FieldType { get; set; } // Text, Date, etc.

        // Coordinates
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public bool IsLocked { get; set; } = false;
        public string ValidationRulesJson { get; set; } = "{}"; // { "required": true }
    }
}
