using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Common.Models;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Queries
{
    public class GetDistributionsQueryHandler : IRequestHandler<GetDistributionsQuery, PagedResult<DistributionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDistributionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<DistributionDto>> Handle(GetDistributionsQuery request, CancellationToken cancellationToken)
        {
            var distributions = await _unitOfWork.DistributionRepository.GetAllAsync();

            // Apply OrganizationId filter if provided
            if (request.OrganizationId.HasValue)
            {
                distributions = distributions.Where(d => d.OrganizationId == request.OrganizationId.Value).ToList();
            }

            // Apply PrinterId filter if provided
            if (request.PrinterId.HasValue)
            {
                distributions = distributions.Where(d => d.PrinterId == request.PrinterId.Value).ToList();
            }

            // Apply IsActive filter if provided
            if (request.IsActive.HasValue)
            {
                distributions = distributions.Where(d => d.IsActive == request.IsActive.Value).ToList();
            }

            var totalCount = distributions.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Load related entities for DTO
            var organizations = await _unitOfWork.OrganizationRepository.GetAllAsync();
            var printers = await _unitOfWork.PrinterRepository.GetAllAsync();

            var orgDict = organizations.ToDictionary(o => o.Id);
            var printerDict = printers.ToDictionary(p => p.Id);

            // Apply pagination
            var pagedDistributions = distributions
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(d => new DistributionDto
                {
                    Id = d.Id,
                    OrganizationId = d.OrganizationId,
                    OrganizationName = orgDict.ContainsKey(d.OrganizationId) ? orgDict[d.OrganizationId].Name : string.Empty,
                    PrinterId = d.PrinterId,
                    PrinterName = printerDict.ContainsKey(d.PrinterId) ? printerDict[d.PrinterId].Name : string.Empty,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToList();

            return new PagedResult<DistributionDto>
            {
                Items = pagedDistributions,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}

