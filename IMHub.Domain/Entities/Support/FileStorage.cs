using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities.Workflow
{
    // 19. FileStorage (Asset Manager)
    public class FileStorage : BaseEntity
    {
        public int OrganizationId { get; set; }
        [Required]
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = "Image";
        public string FileUrl { get; set; } = string.Empty;
    }
}
