using IMHub.ApplicationLayer.Common.Models;
using IMHub.ApplicationLayer.Features.Organizations.Contents;
using IMHub.ApplicationLayer.Features.Organizations.Contents.Commands;
using IMHub.ApplicationLayer.Features.Organizations.Contents.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [ApiController]
    [Route("api/contents")]
    [Authorize(Roles = "OrgAdmin,Employee")]
    public class ContentsController : BaseController
    {
        public ContentsController(IMediator mediator, ILogger<ContentsController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// Get all contents for the organization
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<ContentDto>>> GetContents([FromQuery] GetContentsQuery query)
        {
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Contents retrieved successfully." }));
        }

        /// <summary>
        /// Get content by ID with field values
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ContentWithFieldsDto>> GetContentById(int id)
        {
            var query = new GetContentByIdQuery { Id = id };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Content retrieved successfully." }));
        }

        /// <summary>
        /// Create a new content (manual entry)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ContentDto>> CreateContent([FromBody] CreateContentCommand command)
        {
            return await HandleRequestAsync(command, (result) => 
                CreatedAtAction(nameof(GetContentById), new { id = result.Id },
                    new { success = true, data = result, message = "Content created successfully." }));
        }

        /// <summary>
        /// Update a content field value (only unlocked fields for employees)
        /// </summary>
        [HttpPut("{contentId}/fields/{templateFieldId}")]
        public async Task<ActionResult<ContentFieldValueDto>> UpdateContentFieldValue(
            int contentId, 
            int templateFieldId, 
            [FromBody] UpdateContentFieldValueCommand command)
        {
            command.ContentId = contentId;
            command.TemplateFieldId = templateFieldId;
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "Field value updated successfully." }));
        }
    }
}

