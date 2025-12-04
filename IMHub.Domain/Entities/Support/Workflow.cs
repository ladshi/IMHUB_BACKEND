using IMHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.Domain.Entities.Workflow
{
    // 17. Workflow (Definition)
    public class Workflow : BaseEntity
    {
        public int OrganizationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StepsJson { get; set; } = "{}";
    }
}
