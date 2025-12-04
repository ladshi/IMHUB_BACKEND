using IMHub.ApplicationLayer.Common.Models;
using IMHub.ApplicationLayer.Features.SuperAdmin.Organizations;
using IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands;
using IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [ApiController]
    [Route("api/superadmin/organizations")]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminOrganizationsController : BaseController
    {
        public SuperAdminOrganizationsController(IMediator mediator, ILogger<SuperAdminOrganizationsController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// Get all organizations with pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<OrganizationDto>>> GetOrganizations([FromQuery] GetOrganizationsQuery query)
        {
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Organizations retrieved successfully." }));
        }

        /// <summary>
        /// Get organization by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationDto>> GetOrganizationById(int id)
        {
            var query = new GetOrganizationByIdQuery { Id = id };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Organization retrieved successfully." }));
        }

        /// <summary>
        /// Create a new organization
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrganizationDto>> CreateOrganization([FromBody] CreateOrganizationCommand command)
        {
            return await HandleRequestAsync(command, (result) => 
                CreatedAtAction(nameof(GetOrganizationById), new { id = result.Id },
                    new { success = true, data = result, message = "Organization created successfully." }));
        }

        /// <summary>
        /// Update an existing organization
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<OrganizationDto>> UpdateOrganization(int id, [FromBody] UpdateOrganizationCommand command)
        {
            command.Id = id;
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "Organization updated successfully." }));
        }

        /// <summary>
        /// Delete an organization (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganization(int id)
        {
            var command = new DeleteOrganizationCommand { Id = id };
            return await HandleRequestAsync(command, "Organization deleted successfully");
        }

        /// <summary>
        /// Approve an organization
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<ActionResult<OrganizationDto>> ApproveOrganization(int id)
        {
            var command = new ApproveOrganizationCommand { Id = id };
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "Organization approved successfully." }));
        }

        /// <summary>
        /// Deactivate an organization
        /// </summary>
        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult<OrganizationDto>> DeactivateOrganization(int id)
        {
            var command = new DeactivateOrganizationCommand { Id = id };
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "Organization deactivated successfully." }));
        }
    }
}

