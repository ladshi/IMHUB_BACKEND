using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Commands
{
    public class DeleteDistributionCommandHandler : IRequestHandler<DeleteDistributionCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDistributionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteDistributionCommand request, CancellationToken cancellationToken)
        {
            var distribution = await _unitOfWork.DistributionRepository.GetByIdAsync(request.Id);
            if (distribution == null)
            {
                throw new KeyNotFoundException($"Distribution with ID {request.Id} not found.");
            }

            // Soft delete
            await _unitOfWork.DistributionRepository.DeleteAsync(distribution);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

