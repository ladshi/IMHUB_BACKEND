using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands
{
    public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrganizationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            // Check if organization name already exists
            if (await _unitOfWork.OrganizationRepository.ExistsByNameAsync(request.Name, cancellationToken))
            {
                throw new InvalidOperationException($"Organization with name '{request.Name}' already exists.");
            }

            // Check if domain already exists
            var existingOrg = await _unitOfWork.OrganizationRepository.GetByNameAsync(request.Domain, cancellationToken);
            if (existingOrg != null && existingOrg.Domain == request.Domain)
            {
                throw new InvalidOperationException($"Organization with domain '{request.Domain}' already exists.");
            }

            var organization = new Organization
            {
                Name = request.Name,
                Domain = request.Domain,
                TenantCode = request.TenantCode,
                PlanType = request.PlanType,
                LimitsJson = request.LimitsJson,
                IsActive = request.IsActive
            };

            await _unitOfWork.OrganizationRepository.AddAsync(organization);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

