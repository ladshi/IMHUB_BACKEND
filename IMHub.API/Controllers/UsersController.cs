using IMHub.ApplicationLayer.Common.Models;
using IMHub.ApplicationLayer.Features.Organizations.Users;
using IMHub.ApplicationLayer.Features.Organizations.Users.Commands;
using IMHub.ApplicationLayer.Features.Organizations.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "OrgAdmin")]
    public class UsersController : BaseController
    {
        public UsersController(IMediator mediator, ILogger<UsersController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// Get all users for the organization with pagination and filters
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<UserDto>>> GetUsers([FromQuery] GetUsersQuery query)
        {
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "Users retrieved successfully." }));
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var query = new GetUserByIdQuery { Id = id };
            return await HandleRequestAsync(query, (result) => 
                Ok(new { success = true, data = result, message = "User retrieved successfully." }));
        }

        /// <summary>
        /// Create a new user (Employee)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserCommand command)
        {
            return await HandleRequestAsync(command, (result) => 
                CreatedAtAction(nameof(GetUserById), new { id = result.Id },
                    new { success = true, data = result, message = "User created successfully." }));
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            return await HandleRequestAsync(command, (result) => 
                Ok(new { success = true, data = result, message = "User updated successfully." }));
        }

        /// <summary>
        /// Delete a user (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var command = new DeleteUserCommand { Id = id };
            return await HandleRequestAsync(command, "User deleted successfully");
        }
    }
}

