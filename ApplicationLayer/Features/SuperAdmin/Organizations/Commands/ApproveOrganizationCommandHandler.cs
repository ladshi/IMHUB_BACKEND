using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands
{
    public class ApproveOrganizationCommandHandler : IRequestHandler<ApproveOrganizationCommand, OrganizationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApproveOrganizationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizationDto> Handle(ApproveOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _unitOfWork.OrganizationRepository.GetByIdAsync(request.Id);
            if (organization == null)
            {
                throw new KeyNotFoundException($"Organization with ID {request.Id} not found.");
            }

            organization.IsActive = true;

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

