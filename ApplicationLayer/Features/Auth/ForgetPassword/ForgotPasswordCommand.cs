using MediatR;

namespace IMHub.ApplicationLayer.Features.Auth.ForgetPassword
{
    public class ForgotPasswordCommand : IRequest<string>
    {
        public string Email { get; set; } = string.Empty;
    }
}
