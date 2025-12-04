using IMHub.ApplicationLayer.Features.SuperAdmin.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : BaseController
    {
        public SuperAdminController(IMediator mediator, ILogger<BaseController> logger)
            : base(mediator, logger)
        {
        }

        [HttpPost("approve-request/{id}")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            return await HandleRequestAsync(
                new ApproveOrganizationCommand { RequestId = id },
                "Organization Approved & User Created"
            );
        }
    }
}
