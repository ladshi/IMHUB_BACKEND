using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Users.Commands
{
    public class DeleteUserCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}


