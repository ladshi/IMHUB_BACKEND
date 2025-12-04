using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Users.Commands
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Username { get; set; }
        public bool IsActive { get; set; }
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}


