using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Commands
{
    public class UpdateDistributionCommandHandler : IRequestHandler<UpdateDistributionCommand, DistributionDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDistributionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DistributionDto> Handle(UpdateDistributionCommand request, CancellationToken cancellationToken)
        {
            var distribution = await _unitOfWork.DistributionRepository.GetByIdAsync(request.Id);
            if (distribution == null)
            {
                throw new KeyNotFoundException($"Distribution with ID {request.Id} not found.");
            }

            distribution.IsActive = request.IsActive;

            await _unitOfWork.DistributionRepository.UpdateAsync(distribution);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Load related entities for DTO
            var organization = await _unitOfWork.OrganizationRepository.GetByIdAsync(distribution.OrganizationId);
            var printer = await _unitOfWork.PrinterRepository.GetByIdAsync(distribution.PrinterId);

            return new DistributionDto
            {
                Id = distribution.Id,
                OrganizationId = distribution.OrganizationId,
                OrganizationName = organization?.Name ?? string.Empty,
                PrinterId = distribution.PrinterId,
                PrinterName = printer?.Name ?? string.Empty,
                IsActive = distribution.IsActive,
                CreatedAt = distribution.CreatedAt,
                UpdatedAt = distribution.UpdatedAt
            };
        }
    }
}

