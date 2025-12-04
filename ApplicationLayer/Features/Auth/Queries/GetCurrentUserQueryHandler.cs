using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Features.Auth.Commands;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Auth.Queries
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, LoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCurrentUserQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            // Check if SuperAdmin
            var superAdmin = await _unitOfWork.PlatformAdminRepository.GetByIdAsync(request.UserId);
            if (superAdmin != null)
            {
                return new LoginResponse
                {
                    Id = superAdmin.Id,
                    Name = superAdmin.Name,
                    Email = superAdmin.Email,
                    Role = "SuperAdmin",
                    OrganizationId = null
                };
            }

            // Check regular user
            var user = await _unitOfWork.UserRepository.GetUserWithRolesAsync(request.UserId, cancellationToken);

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User not found or inactive");
            }

            var roleName = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "Employee";

            return new LoginResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = roleName,
                OrganizationId = user.OrganizationId
            };
        }
    }
}

