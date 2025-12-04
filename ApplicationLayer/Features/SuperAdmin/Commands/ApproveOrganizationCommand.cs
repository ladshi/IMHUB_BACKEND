using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Commands
{
    public class ApproveOrganizationCommand : IRequest<Unit>
    {
        public int RequestId { get; set; }
    }
}
