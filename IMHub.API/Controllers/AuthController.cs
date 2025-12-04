using IMHub.ApplicationLayer.Features.Auth.Commands;
using IMHub.ApplicationLayer.Features.Auth.ForgetPassword;
using IMHub.ApplicationLayer.Features.Auth.Queries;
using IMHub.ApplicationLayer.Features.Auth.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMHub.API.Controllers
{
    public class AuthController : BaseController
    {
        public AuthController(IMediator mediator, ILogger<BaseController> logger)
            : base(mediator, logger)
        {
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
        {
            return await HandleRequestAsync(command);
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterCommand command)
        {
            return await HandleRequestAsync(command);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<string>> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            return await HandleRequestAsync(command, (result) => Ok(new { message = result }));
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            return await HandleRequestAsync(command, "Password reset successful");
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<LoginResponse>> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            var email = GetCurrentUserEmail();

            if (!userId.HasValue || string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var query = new GetCurrentUserQuery
            {
                UserId = userId.Value,
                Email = email
            };

            return await HandleRequestAsync(query);
        }
    }
}
