using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands
{
    public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrganizationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _unitOfWork.OrganizationRepository.GetByIdAsync(request.Id);
            if (organization == null)
            {
                throw new KeyNotFoundException($"Organization with ID {request.Id} not found.");
            }

            // Check if name is being changed and if new name already exists
            if (organization.Name != request.Name)
            {
                if (await _unitOfWork.OrganizationRepository.ExistsByNameAsync(request.Name, cancellationToken))
                {
                    throw new InvalidOperationException($"Organization with name '{request.Name}' already exists.");
                }
            }

            organization.Name = request.Name;
            organization.Domain = request.Domain;
            organization.PlanType = request.PlanType;
            organization.LimitsJson = request.LimitsJson;
            organization.IsActive = request.IsActive;

            await _unitOfWork.OrganizationRepository.UpdateAsync(organization);
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

