using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Commands
{
    public class UpdatePrinterCommandHandler : IRequestHandler<UpdatePrinterCommand, PrinterDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePrinterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PrinterDto> Handle(UpdatePrinterCommand request, CancellationToken cancellationToken)
        {
            var printer = await _unitOfWork.PrinterRepository.GetByIdAsync(request.Id);
            if (printer == null)
            {
                throw new KeyNotFoundException($"Printer with ID {request.Id} not found.");
            }

            // Check if name is being changed and if new name already exists
            if (printer.Name != request.Name)
            {
                var existingPrinter = await _unitOfWork.PrinterRepository.GetByNameAsync(request.Name, cancellationToken);
                if (existingPrinter != null && existingPrinter.Id != request.Id)
                {
                    throw new InvalidOperationException($"Printer with name '{request.Name}' already exists.");
                }
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

            printer.OrganizationId = request.OrganizationId;
            printer.Name = request.Name;
            printer.Location = request.Location;
            printer.Description = request.Description;
            printer.SupportsColor = request.SupportsColor;
            printer.SupportsDuplex = request.SupportsDuplex;
            printer.ApiKey = request.ApiKey;
            printer.IsActive = request.IsActive;

            await _unitOfWork.PrinterRepository.UpdateAsync(printer);
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

