using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Users.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Only OrgAdmin can create users
            if (_currentUserService.Role != "OrgAdmin")
            {
                throw new UnauthorizedAccessException("Only Organization Admins can create users.");
            }

            // Check if email already exists
            if (await _unitOfWork.UserRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                throw new InvalidOperationException("Email already registered.");
            }

            // Validate roles exist
            foreach (var roleId in request.RoleIds)
            {
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    throw new KeyNotFoundException($"Role with ID {roleId} not found.");
                }
            }

            // Create user
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Username = request.Username,
                OrganizationId = organizationId,
                IsActive = true
            };

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Assign roles
            foreach (var roleId in request.RoleIds)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                };
                await _unitOfWork.UserRoleRepository.AddAsync(userRole);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload user with roles
            var createdUser = await _unitOfWork.UserRepository.GetUserWithRolesAsync(user.Id, cancellationToken);
            if (createdUser == null)
            {
                throw new InvalidOperationException("Failed to retrieve created user.");
            }

            return MapToDto(createdUser);
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                OrganizationId = user.OrganizationId,
                OrganizationName = user.Organization?.Name ?? string.Empty,
                Name = user.Name,
                Email = user.Email,
                Username = user.Username,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}


