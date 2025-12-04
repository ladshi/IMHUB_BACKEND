using IMHub.ApplicationLayer.Common.Interfaces.Infrastruture;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Auth.ForgetPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<string> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email, cancellationToken);
            
            // Security: Always return "Success" even if user not found to prevent Email Enumeration
            if (user == null) 
                return "If the email exists, a password reset link has been sent.";

            // 1. Generate Secure Token
            string resetToken = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

            // 2. Save to DB
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15); // Valid for 15 mins
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 3. Send Email
            string resetLink = $"http://localhost:5173/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(resetToken)}";
            string emailBody = $"Click the following link to reset your password: {resetLink}\n\nThis link will expire in 15 minutes.";
            await _emailService.SendEmailAsync(user.Email, "Reset Password - IMHub", emailBody);

            return "If the email exists, a password reset link has been sent.";
        }
    }
}
