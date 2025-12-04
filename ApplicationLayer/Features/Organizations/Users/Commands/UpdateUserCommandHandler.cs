using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Users.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Only OrgAdmin can update users
            if (_currentUserService.Role != "OrgAdmin")
            {
                throw new UnauthorizedAccessException("Only Organization Admins can update users.");
            }

            // Get user
            var user = await _unitOfWork.UserRepository.GetUserWithRolesAsync(request.Id, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.Id} not found.");
            }

            // Ensure user belongs to organization
            if (user.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot update user from another organization.");
            }

            // Update user
            user.Name = request.Name;
            user.Username = request.Username;
            user.IsActive = request.IsActive;

            // Update roles
            if (request.RoleIds.Any())
            {
                // Validate roles exist
                foreach (var roleId in request.RoleIds)
                {
                    var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
                    if (role == null)
                    {
                        throw new KeyNotFoundException($"Role with ID {roleId} not found.");
                    }
                }

                // Remove existing roles
                var existingRoles = await _unitOfWork.UserRoleRepository.GetByUserIdAsync(user.Id, cancellationToken);
                foreach (var existingRole in existingRoles)
                {
                    await _unitOfWork.UserRoleRepository.DeleteAsync(existingRole);
                }

                // Add new roles
                foreach (var roleId in request.RoleIds)
                {
                    var userRole = new Domain.Entities.UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    };
                    await _unitOfWork.UserRoleRepository.AddAsync(userRole);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload user with roles
            var updatedUser = await _unitOfWork.UserRepository.GetUserWithRolesAsync(user.Id, cancellationToken);
            if (updatedUser == null)
            {
                throw new InvalidOperationException("Failed to retrieve updated user.");
            }

            return MapToDto(updatedUser);
        }

        private UserDto MapToDto(Domain.Entities.User user)
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


