using IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateVersions;
using IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateVersions.Commands;
using IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateVersions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [ApiController]
    [Route("api/organizations/templates/{templateId}/versions")]
    [Authorize(Roles = "OrgAdmin")]
    public class OrganizationTemplateVersionsController : BaseController
    {
        public OrganizationTemplateVersionsController(IMediator mediator, ILogger<OrganizationTemplateVersionsController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// Get all versions for a template
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TemplateVersionDto>>> GetTemplateVersions(int templateId)
        {
            var query = new GetTemplateVersionsQuery { TemplateId = templateId };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Template versions retrieved successfully." }));
        }

        /// <summary>
        /// Create a new template version
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TemplateVersionDto>> CreateTemplateVersion(int templateId, [FromBody] CreateTemplateVersionCommand command)
        {
            command.TemplateId = templateId;
            return await HandleRequestAsync(command, (result) => 
                CreatedAtAction(nameof(GetTemplateVersions), new { templateId },
                    new { success = true, data = result, message = "Template version created successfully." }));
        }

        /// <summary>
        /// Set active version for a template
        /// </summary>
        [HttpPost("{versionId}/set-active")]
        public async Task<ActionResult<TemplateVersionDto>> SetActiveVersion(int templateId, int versionId)
        {
            var command = new SetActiveVersionCommand { TemplateId = templateId, VersionId = versionId };
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "Active version set successfully." }));
        }
    }
}

