using IMHub.ApplicationLayer.Common.Models;
using IMHub.ApplicationLayer.Features.Organizations.Sendouts;
using IMHub.ApplicationLayer.Features.Organizations.Sendouts.Commands;
using IMHub.ApplicationLayer.Features.Organizations.Sendouts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [ApiController]
    [Route("api/sendouts")]
    [Authorize(Roles = "OrgAdmin,Employee")]
    public class SendoutsController : BaseController
    {
        public SendoutsController(IMediator mediator, ILogger<SendoutsController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// Get all sendouts for the organization with pagination and filters
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<SendoutDto>>> GetSendouts([FromQuery] GetSendoutsQuery query)
        {
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Sendouts retrieved successfully." }));
        }

        /// <summary>
        /// Get sendout by ID with status history
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SendoutWithHistoryDto>> GetSendoutById(int id)
        {
            var query = new GetSendoutByIdQuery { Id = id };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Sendout retrieved successfully." }));
        }

        /// <summary>
        /// Get status history for a sendout
        /// </summary>
        [HttpGet("{sendoutId}/history")]
        public async Task<ActionResult<List<SendoutStatusHistoryDto>>> GetSendoutStatusHistory(int sendoutId)
        {
            var query = new GetSendoutStatusHistoryQuery { SendoutId = sendoutId };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Status history retrieved successfully." }));
        }

        /// <summary>
        /// Create a new sendout
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SendoutDto>> CreateSendout([FromBody] CreateSendoutCommand command)
        {
            return await HandleRequestAsync(command, (result) => 
                CreatedAtAction(nameof(GetSendoutById), new { id = result.Id },
                    new { success = true, data = result, message = "Sendout created successfully." }));
        }

        /// <summary>
        /// Send sendout to printer API
        /// </summary>
        [HttpPost("{sendoutId}/send-to-printer")]
        public async Task<ActionResult<SendoutDto>> SendToPrinter(int sendoutId)
        {
            var command = new SendToPrinterCommand { SendoutId = sendoutId };
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "Sendout sent to printer successfully." }));
        }

        /// <summary>
        /// Update sendout status
        /// </summary>
        [HttpPut("{sendoutId}/status")]
        public async Task<ActionResult<SendoutDto>> UpdateSendoutStatus(int sendoutId, [FromBody] UpdateSendoutStatusCommand command)
        {
            command.SendoutId = sendoutId;
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "Sendout status updated successfully." }));
        }
    }
}

