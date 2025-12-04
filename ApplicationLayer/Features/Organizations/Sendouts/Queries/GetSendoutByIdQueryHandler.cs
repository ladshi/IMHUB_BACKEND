using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Queries
{
    public class GetSendoutByIdQueryHandler : IRequestHandler<GetSendoutByIdQuery, SendoutWithHistoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetSendoutByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SendoutWithHistoryDto> Handle(GetSendoutByIdQuery request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Get sendout by ID
            var sendout = await _unitOfWork.SendoutRepository.GetByIdAsync(request.Id);

            if (sendout == null)
            {
                throw new KeyNotFoundException($"Sendout with ID {request.Id} not found.");
            }

            // Ensure sendout belongs to user's organization
            if (sendout.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access sendout from another organization.");
            }

            // Get status history
            var history = await _unitOfWork.SendoutStatusHistoryRepository
                .GetBySendoutIdAsync(sendout.Id, cancellationToken);

            // Get user names for history entries
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

            return new SendoutWithHistoryDto
            {
                Sendout = MapToDto(sendout),
                History = historyDtos
            };
        }

        private SendoutDto MapToDto(Domain.Entities.Sendout sendout)
        {
            return new SendoutDto
            {
                Id = sendout.Id,
                OrganizationId = sendout.OrganizationId,
                OrganizationName = sendout.Organization?.Name ?? string.Empty,
                ContentId = sendout.ContentId,
                ContentName = sendout.Content?.Name ?? string.Empty,
                PrinterId = sendout.PrinterId,
                PrinterName = sendout.Printer?.Name ?? string.Empty,
                JobReference = sendout.JobReference,
                CurrentStatus = sendout.CurrentStatus,
                CurrentStatusName = sendout.CurrentStatus.ToString(),
                TargetDate = sendout.TargetDate,
                CreatedAt = sendout.CreatedAt,
                UpdatedAt = sendout.UpdatedAt
            };
        }
    }
}

