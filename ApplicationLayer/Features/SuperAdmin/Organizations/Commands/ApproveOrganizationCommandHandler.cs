using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands
{
    public class ApproveOrganizationCommandHandler : IRequestHandler<ApproveOrganizationCommand, OrganizationDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ApproveOrganizationCommandHandler> _logger;

        public ApproveOrganizationCommandHandler(IUnitOfWork unitOfWork, ILogger<ApproveOrganizationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<OrganizationDto> Handle(ApproveOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _unitOfWork.OrganizationRepository.GetByIdAsync(request.Id);
            if (organization == null)
            {
                throw new KeyNotFoundException($"Organization with ID {request.Id} not found.");
            }

            _logger.LogInformation("Approving organization: {OrganizationId} - {OrganizationName}", organization.Id, organization.Name);

            // Activate organization
            organization.IsActive = true;
            await _unitOfWork.OrganizationRepository.UpdateAsync(organization);

            // Activate all users in this organization using bulk update
            _logger.LogInformation("Activating all inactive users for organization {OrganizationId}", organization.Id);
            
            // First, get count of inactive users before activation
            var usersBefore = await _unitOfWork.UserRepository.GetByOrganizationIdAsync(organization.Id, cancellationToken);
            var inactiveUsersBefore = usersBefore.Where(u => !u.IsActive && !u.IsDeleted).ToList();
            _logger.LogInformation("Found {Count} inactive users before activation", inactiveUsersBefore.Count);
            
            // Perform bulk activation using ExecuteUpdateAsync (direct SQL update)
            var usersActivated = await _unitOfWork.UserRepository.ActivateUsersByOrganizationIdAsync(organization.Id, cancellationToken);
            _logger.LogInformation("ExecuteUpdateAsync returned: {Count} users activated", usersActivated);
            
            // Save organization changes
            var savedChanges = await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Saved {Count} changes to database (organization update)", savedChanges);

            // If ExecuteUpdateAsync didn't activate all users, use fallback method
            if (usersActivated == 0 && inactiveUsersBefore.Any())
            {
                _logger.LogWarning("ExecuteUpdateAsync returned 0, using fallback method to activate users");
                foreach (var user in inactiveUsersBefore)
                {
                    user.IsActive = true;
                    await _unitOfWork.UserRepository.UpdateAsync(user);
                    _logger.LogInformation("Fallback: Activated user {UserId} - {UserEmail}", user.Id, user.Email);
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Fallback method activated {Count} users", inactiveUsersBefore.Count);
            }

            // Wait a moment for database to commit
            await Task.Delay(200, cancellationToken);

            // Verify activation by getting fresh data from database (clear cache first)
            var usersAfter = await _unitOfWork.UserRepository.GetByOrganizationIdAsync(organization.Id, cancellationToken);
            var activeUsers = usersAfter.Where(u => u.IsActive && !u.IsDeleted).ToList();
            var inactiveUsers = usersAfter.Where(u => !u.IsActive && !u.IsDeleted).ToList();
            
            _logger.LogInformation("Verification after activation:");
            _logger.LogInformation("  - Active users: {ActiveCount}", activeUsers.Count);
            _logger.LogInformation("  - Inactive users: {InactiveCount}", inactiveUsers.Count);

            // Log each active user for audit
            foreach (var user in activeUsers)
            {
                _logger.LogInformation("✓ Active user: {UserId} - {UserEmail}", user.Id, user.Email);
            }
            
            // Log any remaining inactive users (this should not happen)
            if (inactiveUsers.Any())
            {
                _logger.LogError("ERROR: {Count} users are still inactive after activation attempt!", inactiveUsers.Count);
                foreach (var user in inactiveUsers)
                {
                    _logger.LogError("⚠ Still inactive: {UserId} - {UserEmail} (IsActive: {IsActive}, IsDeleted: {IsDeleted})", 
                        user.Id, user.Email, user.IsActive, user.IsDeleted);
                }
            }

            _logger.LogInformation("Organization {OrganizationId} approved successfully. {UserCount} users activated.", 
                organization.Id, activeUsers.Count);

            return new OrganizationDto
            {
                Id = organization.Id,
                Name = organization.Name,
                Domain = organization.Domain,
                TenantCode = organization.TenantCode,
                PlanType = organization.PlanType.ToString(),
                LimitsJson = organization.LimitsJson,
                IsActive = organization.IsActive,
                CreatedAt = organization.CreatedAt,
                UpdatedAt = organization.UpdatedAt
            };
        }
    }
}

