using IMHub.ApplicationLayer.Common.Models;
using IMHub.ApplicationLayer.Features.Organizations.Templates;
using IMHub.ApplicationLayer.Features.Organizations.Templates.Commands;
using IMHub.ApplicationLayer.Features.Organizations.Templates.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [ApiController]
    [Route("api/organizations/templates")]
    [Authorize(Roles = "OrgAdmin")]
    public class OrganizationTemplatesController : BaseController
    {
        public OrganizationTemplatesController(IMediator mediator, ILogger<OrganizationTemplatesController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// Get all templates for the organization
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<TemplateDto>>> GetTemplates([FromQuery] GetTemplatesQuery query)
        {
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Templates retrieved successfully." }));
        }

        /// <summary>
        /// Get template by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TemplateDto>> GetTemplateById(int id)
        {
            var query = new GetTemplateByIdQuery { Id = id };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Template retrieved successfully." }));
        }

        /// <summary>
        /// Create a new template
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TemplateDto>> CreateTemplate([FromBody] CreateTemplateCommand command)
        {
            return await HandleRequestAsync(command, (result) => 
                CreatedAtAction(nameof(GetTemplateById), new { id = result.Id },
                    new { success = true, data = result, message = "Template created successfully." }));
        }

        /// <summary>
        /// Update an existing template
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TemplateDto>> UpdateTemplate(int id, [FromBody] UpdateTemplateCommand command)
        {
            command.Id = id;
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "Template updated successfully." }));
        }

        /// <summary>
        /// Delete a template (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var command = new DeleteTemplateCommand { Id = id };
            return await HandleRequestAsync(command, "Template deleted successfully");
        }
    }
}

