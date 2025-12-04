using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Commands
{
    public class CreatePrinterCommandHandler : IRequestHandler<CreatePrinterCommand, PrinterDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePrinterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PrinterDto> Handle(CreatePrinterCommand request, CancellationToken cancellationToken)
        {
            // Check if printer name already exists
            var existingPrinter = await _unitOfWork.PrinterRepository.GetByNameAsync(request.Name, cancellationToken);
            if (existingPrinter != null)
            {
                throw new InvalidOperationException($"Printer with name '{request.Name}' already exists.");
            }

            // Validate organization exists if OrganizationId is provided
            if (request.OrganizationId.HasValue)
            {
                var organization = await _unitOfWork.OrganizationRepository.GetByIdAsync(request.OrganizationId.Value);
                if (organization == null)
                {
                    throw new KeyNotFoundException($"Organization with ID {request.OrganizationId.Value} not found.");
                }
            }

            var printer = new Printer
            {
                OrganizationId = request.OrganizationId,
                Name = request.Name,
                Location = request.Location,
                Description = request.Description,
                SupportsColor = request.SupportsColor,
                SupportsDuplex = request.SupportsDuplex,
                ApiKey = request.ApiKey,
                IsActive = request.IsActive
            };

            await _unitOfWork.PrinterRepository.AddAsync(printer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

