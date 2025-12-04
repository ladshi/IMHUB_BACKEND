using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Users.Commands
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Username { get; set; }
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}


