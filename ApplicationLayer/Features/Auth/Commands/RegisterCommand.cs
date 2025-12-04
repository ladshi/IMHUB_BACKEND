using MediatR;

namespace IMHub.ApplicationLayer.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<RegisterResponse>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

