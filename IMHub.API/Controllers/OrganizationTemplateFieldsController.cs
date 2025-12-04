using IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateFields;
using IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateFields.Commands;
using IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateFields.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [ApiController]
    [Route("api/organizations/templates/{templateId}/versions/{templateVersionId}/pages/{templatePageId}/fields")]
    [Authorize(Roles = "OrgAdmin")]
    public class OrganizationTemplateFieldsController : BaseController
    {
        public OrganizationTemplateFieldsController(IMediator mediator, ILogger<OrganizationTemplateFieldsController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// Get all fields for a template page
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TemplateFieldDto>>> GetTemplateFields(int templatePageId, [FromQuery] bool? isLocked)
        {
            var query = new GetTemplateFieldsQuery { TemplatePageId = templatePageId, IsLocked = isLocked };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Template fields retrieved successfully." }));
        }

        /// <summary>
        /// Create a new template field (editable placeholder)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TemplateFieldDto>> CreateTemplateField(int templatePageId, [FromBody] CreateTemplateFieldCommand command)
        {
            command.TemplatePageId = templatePageId;
            return await HandleRequestAsync(command, (result) => 
                CreatedAtAction(nameof(GetTemplateFields), new { templatePageId },
                    new { success = true, data = result, message = "Template field created successfully." }));
        }

        /// <summary>
        /// Update an existing template field
        /// </summary>
        [HttpPut("{fieldId}")]
        public async Task<ActionResult<TemplateFieldDto>> UpdateTemplateField(int fieldId, [FromBody] UpdateTemplateFieldCommand command)
        {
            command.Id = fieldId;
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "Template field updated successfully." }));
        }
    }
}

