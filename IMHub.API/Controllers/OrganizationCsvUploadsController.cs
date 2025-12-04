using IMHub.ApplicationLayer.Common.Models;
using IMHub.ApplicationLayer.Features.Organizations.CsvUploads;
using IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Commands;
using IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Queries;
using IMHub.ApplicationLayer.Features.Organizations.Contents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [ApiController]
    [Route("api/organizations/csv-uploads")]
    [Authorize(Roles = "OrgAdmin,Employee")]
    public class OrganizationCsvUploadsController : BaseController
    {
        public OrganizationCsvUploadsController(IMediator mediator, ILogger<OrganizationCsvUploadsController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// Get all CSV uploads for the organization
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<CsvUploadDto>>> GetCsvUploads([FromQuery] GetCsvUploadsQuery query)
        {
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "CSV uploads retrieved successfully." }));
        }

        /// <summary>
        /// Upload a CSV file
        /// </summary>
        [HttpPost("templates/{templateId}")]
        public async Task<ActionResult<CsvUploadDto>> UploadCsv(int templateId, [FromForm] IFormFile file)
        {
            var command = new UploadCsvCommand
            {
                TemplateId = templateId,
                File = file
            };
            return await HandleRequestAsync(command, (result) => 
                CreatedAtAction(nameof(GetCsvUploads), new { },
                    new { success = true, data = result, message = "CSV file uploaded successfully." }));
        }

        /// <summary>
        /// Map CSV columns to template fields
        /// </summary>
        [HttpPost("{csvUploadId}/map-fields")]
        public async Task<ActionResult<CsvUploadDto>> MapCsvFields(int csvUploadId, [FromBody] MapCsvFieldsCommand command)
        {
            command.CsvUploadId = csvUploadId;
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "CSV fields mapped successfully." }));
        }

        /// <summary>
        /// Generate content from CSV
        /// </summary>
        [HttpPost("{csvUploadId}/generate-content")]
        public async Task<ActionResult<List<ContentDto>>> GenerateContentFromCsv(int csvUploadId, [FromBody] GenerateContentFromCsvCommand command)
        {
            command.CsvUploadId = csvUploadId;
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = $"Generated {result.Count} content items successfully." }));
        }
    }
}

