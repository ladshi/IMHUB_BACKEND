using MediatR;
using IMHub.ApplicationLayer.Features.Auth.Commands;

namespace IMHub.ApplicationLayer.Features.Auth.Queries
{
    public class GetCurrentUserQuery : IRequest<LoginResponse>
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}

