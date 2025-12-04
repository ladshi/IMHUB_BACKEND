using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Queries
{
    public class GetDistributionByIdQueryHandler : IRequestHandler<GetDistributionByIdQuery, DistributionDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDistributionByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DistributionDto> Handle(GetDistributionByIdQuery request, CancellationToken cancellationToken)
        {
            var distribution = await _unitOfWork.DistributionRepository.GetByIdAsync(request.Id);
            if (distribution == null)
            {
                throw new KeyNotFoundException($"Distribution with ID {request.Id} not found.");
            }

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

