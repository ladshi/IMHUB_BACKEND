using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using IMHub.Domain.Enums;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Auth.Commands
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists
            if (await _unitOfWork.UserRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                throw new InvalidOperationException("Email already registered");
            }

            // Check if organization name already exists
            var existingOrg = await _unitOfWork.OrganizationRepository.GetByNameAsync(request.OrganizationName, cancellationToken);

            Organization organization;
            if (existingOrg == null)
            {
                // Create new organization
                organization = new Organization
                {
                    Name = request.OrganizationName,
                    Domain = request.OrganizationName.ToLower().Replace(" ", "-"),
                    TenantCode = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                    PlanType = PlanType.Free,
                    IsActive = false // Requires admin approval
                };
                await _unitOfWork.OrganizationRepository.AddAsync(organization);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else
            {
                organization = existingOrg;
            }

            // Get Employee role (assuming it exists from seed)
            var employeeRole = await _unitOfWork.RoleRepository.GetByNameAsync("Employee", cancellationToken);
            if (employeeRole == null)
            {
                throw new InvalidOperationException("Employee role not found. Please contact administrator.");
            }

            // Create user
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                OrganizationId = organization.Id,
                IsActive = false // Requires admin approval
            };

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Assign Employee role
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = employeeRole.Id
            };
            await _unitOfWork.UserRoleRepository.AddAsync(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                OrganizationId = organization.Id,
                Message = "Registration successful. Please wait for admin approval."
            };
        }
    }
}

