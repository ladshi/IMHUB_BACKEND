using IMHub.ApplicationLayer.Common.Models;
using IMHub.ApplicationLayer.Features.SuperAdmin.Distributions;
using IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Commands;
using IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Queries;
using IMHub.ApplicationLayer.Features.SuperAdmin.Organizations;
using IMHub.ApplicationLayer.Features.SuperAdmin.Printers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [ApiController]
    [Route("api/superadmin/distributions")]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminDistributionsController : BaseController
    {
        public SuperAdminDistributionsController(IMediator mediator, ILogger<SuperAdminDistributionsController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// Get all distributions with pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<DistributionDto>>> GetDistributions([FromQuery] GetDistributionsQuery query)
        {
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Distributions retrieved successfully." }));
        }

        /// <summary>
        /// Get distribution by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DistributionDto>> GetDistributionById(int id)
        {
            var query = new GetDistributionByIdQuery { Id = id };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Distribution retrieved successfully." }));
        }

        /// <summary>
        /// Create a new distribution (link printer to organization)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DistributionDto>> CreateDistribution([FromBody] CreateDistributionCommand command)
        {
            return await HandleRequestAsync(command, (result) => 
                CreatedAtAction(nameof(GetDistributionById), new { id = result.Id },
                    new { success = true, data = result, message = "Distribution created successfully." }));
        }

        /// <summary>
        /// Update an existing distribution
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<DistributionDto>> UpdateDistribution(int id, [FromBody] UpdateDistributionCommand command)
        {
            command.Id = id;
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "Distribution updated successfully." }));
        }

        /// <summary>
        /// Delete a distribution (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistribution(int id)
        {
            var command = new DeleteDistributionCommand { Id = id };
            return await HandleRequestAsync(command, "Distribution deleted successfully");
        }

        /// <summary>
        /// Get printers by organization ID
        /// </summary>
        [HttpGet("organizations/{organizationId}/printers")]
        public async Task<ActionResult<List<PrinterDto>>> GetPrintersByOrganization(int organizationId)
        {
            var query = new GetPrintersByOrganizationQuery { OrganizationId = organizationId };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Printers retrieved successfully." }));
        }

        /// <summary>
        /// Get organizations by printer ID
        /// </summary>
        [HttpGet("printers/{printerId}/organizations")]
        public async Task<ActionResult<List<OrganizationDto>>> GetOrganizationsByPrinter(int printerId)
        {
            var query = new GetOrganizationsByPrinterQuery { PrinterId = printerId };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Organizations retrieved successfully." }));
        }
    }
}

