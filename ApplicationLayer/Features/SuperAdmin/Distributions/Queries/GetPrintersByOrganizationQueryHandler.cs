using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Features.SuperAdmin.Printers;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Queries
{
    public class GetPrintersByOrganizationQueryHandler : IRequestHandler<GetPrintersByOrganizationQuery, List<PrinterDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPrintersByOrganizationQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PrinterDto>> Handle(GetPrintersByOrganizationQuery request, CancellationToken cancellationToken)
        {
            var distributions = await _unitOfWork.DistributionRepository.GetByOrganizationIdAsync(request.OrganizationId, cancellationToken);
            
            var printerIds = distributions.Select(d => d.PrinterId).ToList();
            var allPrinters = await _unitOfWork.PrinterRepository.GetAllAsync();
            
            var printers = allPrinters
                .Where(p => printerIds.Contains(p.Id))
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

            return printers;
        }
    }
}

