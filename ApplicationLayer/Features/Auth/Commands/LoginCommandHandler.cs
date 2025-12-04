using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenGenerator _jwtGenerator;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IJwtTokenGenerator jwtGenerator)
        {
            _unitOfWork = unitOfWork;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 1. Check in PlatformAdmins (Super Admin)
            var superAdmin = await _unitOfWork.PlatformAdminRepository.GetByEmailOrNameAsync(request.Email, cancellationToken);

            if (superAdmin != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(request.Password, superAdmin.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Invalid Credentials");
                }

                var token = _jwtGenerator.GenerateToken(superAdmin.Id, superAdmin.Email, "SuperAdmin", null);

                return new LoginResponse
                {
                    Id = superAdmin.Id,
                    Name = superAdmin.Name,
                    Email = superAdmin.Email,
                    Role = "SuperAdmin",
                    Token = token
                };
            }

            // 2. Check in Organization Users
            var user = await _unitOfWork.UserRepository.GetUserByEmailOrUsernameAsync(request.Email, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid Credentials");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Account is inactive.");
            }

            // Verify Password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid Credentials");
            }

            // Get Role (Assuming 1 Primary Role for MVP Login)
            var roleName = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "Employee";

            // Generate Token
            var userToken = _jwtGenerator.GenerateToken(user.Id, user.Email, roleName, user.OrganizationId);

            return new LoginResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = roleName,
                OrganizationId = user.OrganizationId,
                Token = userToken
            };
        }
    }
}
