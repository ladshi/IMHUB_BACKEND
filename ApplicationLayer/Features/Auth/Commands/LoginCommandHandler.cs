using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IMHub.ApplicationLayer.Features.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IJwtTokenGenerator jwtGenerator,
            ILogger<LoginCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _jwtGenerator = jwtGenerator;
            _logger = logger;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            // 1. Check in PlatformAdmins (Super Admin)
            var superAdmin = await _unitOfWork.PlatformAdminRepository.GetByEmailOrNameAsync(request.Email, cancellationToken);

            if (superAdmin != null)
            {
                _logger.LogInformation("SuperAdmin found: {Email}", superAdmin.Email);
                
                if (string.IsNullOrEmpty(superAdmin.PasswordHash))
                {
                    _logger.LogError("SuperAdmin password hash is null or empty");
                    throw new UnauthorizedAccessException("Invalid Credentials");
                }

                var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, superAdmin.PasswordHash);
                
                if (!passwordValid)
                {
                    _logger.LogWarning("Invalid password for SuperAdmin: {Email}", superAdmin.Email);
                    throw new UnauthorizedAccessException("Invalid Credentials");
                }

                _logger.LogInformation("SuperAdmin login successful: {Email}", superAdmin.Email);
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

            _logger.LogInformation("SuperAdmin not found, checking regular users");

            // 2. Check in Organization Users
            // First check if user exists (including deleted ones for better error message)
            var userExists = await _unitOfWork.UserRepository.ExistsByEmailAsync(request.Email, cancellationToken);
            
            if (!userExists)
            {
                _logger.LogWarning("User not found for email: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid Credentials");
            }

            // Get user with all details (global query filter will exclude deleted users)
            var user = await _unitOfWork.UserRepository.GetUserByEmailOrUsernameAsync(request.Email, cancellationToken);

            if (user == null)
            {
                // User exists but is deleted (filtered out by global query filter)
                _logger.LogWarning("User found but is deleted for email: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid Credentials");
            }

            // Check if user is active BEFORE password verification for better UX
            if (!user.IsActive)
            {
                _logger.LogWarning("Inactive account login attempt: {Email}. OrganizationId: {OrgId}, OrganizationIsActive: {OrgActive}", 
                    request.Email, user.OrganizationId, user.Organization?.IsActive ?? false);
                throw new UnauthorizedAccessException("Your account is pending admin approval. Please wait for administrator to approve your organization before logging in.");
            }

            // Verify Password
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogError("User password hash is null or empty for: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid Credentials");
            }

            var userPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            
            if (!userPasswordValid)
            {
                _logger.LogWarning("Invalid password for user: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid Credentials");
            }

            // Get Role (Assuming 1 Primary Role for MVP Login)
            var roleName = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "Employee";

            _logger.LogInformation("User login successful: {Email}, Role: {Role}", request.Email, roleName);

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
