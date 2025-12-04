using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Commands
{
    public class CreateDistributionCommandHandler : IRequestHandler<CreateDistributionCommand, DistributionDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDistributionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DistributionDto> Handle(CreateDistributionCommand request, CancellationToken cancellationToken)
        {
            // Check if distribution already exists
            if (await _unitOfWork.DistributionRepository.ExistsAsync(request.OrganizationId, request.PrinterId, cancellationToken))
            {
                throw new InvalidOperationException(
                    $"Distribution between Organization {request.OrganizationId} and Printer {request.PrinterId} already exists.");
            }

            // Validate organization exists
            var organization = await _unitOfWork.OrganizationRepository.GetByIdAsync(request.OrganizationId);
            if (organization == null)
            {
                throw new KeyNotFoundException($"Organization with ID {request.OrganizationId} not found.");
            }

            // Validate printer exists
            var printer = await _unitOfWork.PrinterRepository.GetByIdAsync(request.PrinterId);
            if (printer == null)
            {
                throw new KeyNotFoundException($"Printer with ID {request.PrinterId} not found.");
            }

            var distribution = new Distribution
            {
                OrganizationId = request.OrganizationId,
                PrinterId = request.PrinterId,
                IsActive = request.IsActive
            };

            await _unitOfWork.DistributionRepository.AddAsync(distribution);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new DistributionDto
            {
                Id = distribution.Id,
                OrganizationId = distribution.OrganizationId,
                OrganizationName = organization.Name,
                PrinterId = distribution.PrinterId,
                PrinterName = printer.Name,
                IsActive = distribution.IsActive,
                CreatedAt = distribution.CreatedAt,
                UpdatedAt = distribution.UpdatedAt
            };
        }
    }
}

