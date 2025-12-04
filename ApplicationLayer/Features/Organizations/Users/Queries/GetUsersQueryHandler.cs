using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Common.Models;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Users.Queries
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetUsersQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Get users for organization only
            var users = await _unitOfWork.UserRepository.GetByOrganizationIdAsync(organizationId, cancellationToken);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                users = users.Where(u =>
                    u.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (u.Username != null && u.Username.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (request.IsActive.HasValue)
            {
                users = users.Where(u => u.IsActive == request.IsActive.Value).ToList();
            }

            if (request.RoleId.HasValue)
            {
                users = users.Where(u => u.UserRoles.Any(ur => ur.RoleId == request.RoleId.Value)).ToList();
            }

            var totalCount = users.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Apply pagination
            var pagedUsers = users
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var userDtos = pagedUsers.Select(MapToDto).ToList();

            return new PagedResult<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
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


