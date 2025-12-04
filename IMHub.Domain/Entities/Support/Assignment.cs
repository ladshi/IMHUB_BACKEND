using IMHub.Domain.Common;
using IMHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities.Workflow
{
    // 18. Assignment (Task)
    public class Assignment : BaseEntity
    {
        public int TemplateId { get; set; }
        public int AssigneeUserId { get; set; }
        public int AssignedByUserId { get; set; }
        public DateTime DueDate { get; set; }
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Pending;
    }
}
