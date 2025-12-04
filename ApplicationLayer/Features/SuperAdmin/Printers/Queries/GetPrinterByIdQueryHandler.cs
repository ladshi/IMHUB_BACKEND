using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Queries
{
    public class GetPrinterByIdQueryHandler : IRequestHandler<GetPrinterByIdQuery, PrinterDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPrinterByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PrinterDto> Handle(GetPrinterByIdQuery request, CancellationToken cancellationToken)
        {
            var printer = await _unitOfWork.PrinterRepository.GetByIdAsync(request.Id);
            if (printer == null)
            {
                throw new KeyNotFoundException($"Printer with ID {request.Id} not found.");
            }

            return new PrinterDto
            {
                Id = printer.Id,
                OrganizationId = printer.OrganizationId,
                Name = printer.Name,
                Location = printer.Location,
                Description = printer.Description,
                SupportsColor = printer.SupportsColor,
                SupportsDuplex = printer.SupportsDuplex,
                ApiKey = printer.ApiKey,
                IsActive = printer.IsActive,
                CreatedAt = printer.CreatedAt,
                UpdatedAt = printer.UpdatedAt
            };
        }
    }
}

