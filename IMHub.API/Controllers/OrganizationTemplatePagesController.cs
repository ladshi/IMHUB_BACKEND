using IMHub.ApplicationLayer.Features.Organizations.Templates.TemplatePages;
using IMHub.ApplicationLayer.Features.Organizations.Templates.TemplatePages.Commands;
using IMHub.ApplicationLayer.Features.Organizations.Templates.TemplatePages.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [ApiController]
    [Route("api/organizations/templates/{templateId}/versions/{templateVersionId}/pages")]
    [Authorize(Roles = "OrgAdmin")]
    public class OrganizationTemplatePagesController : BaseController
    {
        public OrganizationTemplatePagesController(IMediator mediator, ILogger<OrganizationTemplatePagesController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// Get all pages for a template version
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TemplatePageDto>>> GetTemplatePages(int templateVersionId)
        {
            var query = new GetTemplatePagesQuery { TemplateVersionId = templateVersionId };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Template pages retrieved successfully." }));
        }

        /// <summary>
        /// Create a new template page
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TemplatePageDto>> CreateTemplatePage(int templateVersionId, [FromBody] CreateTemplatePageCommand command)
        {
            command.TemplateVersionId = templateVersionId;
            return await HandleRequestAsync(command, (result) => 
                CreatedAtAction(nameof(GetTemplatePages), new { templateVersionId },
                    new { success = true, data = result, message = "Template page created successfully." }));
        }
    }
}

