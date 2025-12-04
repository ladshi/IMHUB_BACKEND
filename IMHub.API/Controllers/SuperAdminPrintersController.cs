using IMHub.ApplicationLayer.Common.Models;
using IMHub.ApplicationLayer.Features.SuperAdmin.Printers;
using IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Commands;
using IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [ApiController]
    [Route("api/superadmin/printers")]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminPrintersController : BaseController
    {
        public SuperAdminPrintersController(IMediator mediator, ILogger<SuperAdminPrintersController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// Get all printers with pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<PrinterDto>>> GetPrinters([FromQuery] GetPrintersQuery query)
        {
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Printers retrieved successfully." }));
        }

        /// <summary>
        /// Get printer by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PrinterDto>> GetPrinterById(int id)
        {
            var query = new GetPrinterByIdQuery { Id = id };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Printer retrieved successfully." }));
        }

        /// <summary>
        /// Create a new printer
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PrinterDto>> CreatePrinter([FromBody] CreatePrinterCommand command)
        {
            return await HandleRequestAsync(command, (result) => 
                CreatedAtAction(nameof(GetPrinterById), new { id = result.Id },
                    new { success = true, data = result, message = "Printer created successfully." }));
        }

        /// <summary>
        /// Update an existing printer
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PrinterDto>> UpdatePrinter(int id, [FromBody] UpdatePrinterCommand command)
        {
            command.Id = id;
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "Printer updated successfully." }));
        }

        /// <summary>
        /// Delete a printer (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrinter(int id)
        {
            var command = new DeletePrinterCommand { Id = id };
            return await HandleRequestAsync(command, "Printer deleted successfully");
        }
    }
}

