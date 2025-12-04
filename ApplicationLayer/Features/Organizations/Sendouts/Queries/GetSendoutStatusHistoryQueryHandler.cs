using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Queries
{
    public class GetSendoutStatusHistoryQueryHandler : IRequestHandler<GetSendoutStatusHistoryQuery, List<SendoutStatusHistoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetSendoutStatusHistoryQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<List<SendoutStatusHistoryDto>> Handle(GetSendoutStatusHistoryQuery request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Validate sendout exists and belongs to organization
            var sendout = await _unitOfWork.SendoutRepository.GetByIdAsync(request.SendoutId);
            if (sendout == null)
            {
                throw new KeyNotFoundException($"Sendout with ID {request.SendoutId} not found.");
            }

            if (sendout.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access sendout from another organization.");
            }

            // Get status history
            var history = await _unitOfWork.SendoutStatusHistoryRepository
                .GetBySendoutIdAsync(request.SendoutId, cancellationToken);

            // Map to DTOs
            var historyDtos = new List<SendoutStatusHistoryDto>();
            foreach (var historyEntry in history)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(historyEntry.UpdatedByUserId);
                historyDtos.Add(new SendoutStatusHistoryDto
                {
                    Id = historyEntry.Id,
                    SendoutId = historyEntry.SendoutId,
                    JobReference = sendout.JobReference,
                    Status = historyEntry.Status,
                    StatusName = historyEntry.Status.ToString(),
                    Notes = historyEntry.Notes,
                    UpdatedByUserId = historyEntry.UpdatedByUserId,
                    UpdatedByUserName = user?.Email ?? "Unknown",
                    CreatedAt = historyEntry.CreatedAt
                });
            }

            return historyDtos;
        }
    }
}

