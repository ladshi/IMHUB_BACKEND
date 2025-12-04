using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Commands
{
    public class DeletePrinterCommandHandler : IRequestHandler<DeletePrinterCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePrinterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeletePrinterCommand request, CancellationToken cancellationToken)
        {
            var printer = await _unitOfWork.PrinterRepository.GetByIdAsync(request.Id);
            if (printer == null)
            {
                throw new KeyNotFoundException($"Printer with ID {request.Id} not found.");
            }

            // Soft delete
            await _unitOfWork.PrinterRepository.DeleteAsync(printer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

