using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Common.Models;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Queries
{
    public class GetPrintersQueryHandler : IRequestHandler<GetPrintersQuery, PagedResult<PrinterDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPrintersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<PrinterDto>> Handle(GetPrintersQuery request, CancellationToken cancellationToken)
        {
            var printers = await _unitOfWork.PrinterRepository.GetAllAsync();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                printers = printers.Where(p =>
                    p.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Location.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null && p.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Apply OrganizationId filter if provided
            if (request.OrganizationId.HasValue)
            {
                printers = printers.Where(p => p.OrganizationId == request.OrganizationId.Value).ToList();
            }

            // Apply IsActive filter if provided
            if (request.IsActive.HasValue)
            {
                printers = printers.Where(p => p.IsActive == request.IsActive.Value).ToList();
            }

            var totalCount = printers.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Apply pagination
            var pagedPrinters = printers
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new PrinterDto
                {
                    Id = p.Id,
                    OrganizationId = p.OrganizationId,
                    Name = p.Name,
                    Location = p.Location,
                    Description = p.Description,
                    SupportsColor = p.SupportsColor,
                    SupportsDuplex = p.SupportsDuplex,
                    ApiKey = p.ApiKey,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToList();

            return new PagedResult<PrinterDto>
            {
                Items = pagedPrinters,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}

