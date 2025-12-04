using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Users.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public int Id { get; set; }
    }
}


