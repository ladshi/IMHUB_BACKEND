using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IMHub.API.Controllers
{
    /// <summary>
    /// Base controller that provides common functionality for all API controllers.
    /// Inherit from this class to get access to Mediator and helper methods.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IMediator Mediator;
        protected readonly ILogger<BaseController> Logger;

        protected BaseController(
            IMediator mediator,
            ILogger<BaseController> logger)
        {
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the current user ID from JWT claims.
        /// Returns null if user is not authenticated or ID is invalid.
        /// </summary>
        protected int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                             User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return null;
            }

            return userId;
        }

        /// <summary>
        /// Gets the current user email from JWT claims.
        /// Returns null if user is not authenticated or email is not found.
        /// </summary>
        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value ??
                   User.FindFirst("email")?.Value;
        }

        /// <summary>
        /// Gets the current user's role from JWT claims.
        /// Returns null if role is not found.
        /// </summary>
        protected string? GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ??
                   User.FindFirst("role")?.Value;
        }

        /// <summary>
        /// Checks if the current user is authenticated.
        /// </summary>
        protected bool IsAuthenticated()
        {
            return User?.Identity?.IsAuthenticated ?? false;
        }

        /// <summary>
        /// Executes a MediatR command/query with error handling.
        /// Returns appropriate HTTP responses based on exceptions.
        /// </summary>
        protected async Task<ActionResult<TResponse>> HandleRequestAsync<TResponse>(
            IRequest<TResponse> request,
            Func<TResponse, ActionResult<TResponse>>? onSuccess = null)
        {
            try
            {
                var response = await Mediator.Send(request);
                
                if (onSuccess != null)
                {
                    return onSuccess(response);
                }

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                Logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Let the middleware handle unexpected exceptions
                Logger.LogError(ex, "Unexpected error handling request: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Executes a MediatR command/query that returns Unit (void) with error handling.
        /// </summary>
        protected async Task<ActionResult> HandleRequestAsync(
            IRequest<Unit> request,
            string? successMessage = null)
        {
            try
            {
                await Mediator.Send(request);
                
                var message = successMessage ?? "Operation completed successfully";
                return Ok(new { message });
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                Logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Let the middleware handle unexpected exceptions
                Logger.LogError(ex, "Unexpected error handling request: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Returns a standardized success response.
        /// </summary>
        protected ActionResult SuccessResponse(object? data = null, string? message = null)
        {
            if (data == null && message == null)
            {
                return Ok(new { message = "Success" });
            }

            if (data == null)
            {
                return Ok(new { message });
            }

            if (message == null)
            {
                return Ok(data);
            }

            return Ok(new { data, message });
        }

        /// <summary>
        /// Returns a standardized error response.
        /// </summary>
        protected ActionResult ErrorResponse(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, new { message });
        }
    }
}

