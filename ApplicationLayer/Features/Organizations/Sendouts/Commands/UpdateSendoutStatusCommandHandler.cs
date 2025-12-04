using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using IMHub.Domain.Enums;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Commands
{
    public class UpdateSendoutStatusCommandHandler : IRequestHandler<UpdateSendoutStatusCommand, SendoutDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateSendoutStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SendoutDto> Handle(UpdateSendoutStatusCommand request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Get sendout with related entities
            var sendout = await _unitOfWork.SendoutRepository.GetByIdAsync(request.SendoutId);
            if (sendout == null)
            {
                throw new KeyNotFoundException($"Sendout with ID {request.SendoutId} not found.");
            }

            // Ensure sendout belongs to user's organization
            if (sendout.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot update sendout from another organization.");
            }

            // Validate status transition
            if (!IsValidStatusTransition(sendout.CurrentStatus, request.Status))
            {
                throw new InvalidOperationException(
                    $"Invalid status transition from {sendout.CurrentStatus} to {request.Status}.");
            }

            // Update status
            var previousStatus = sendout.CurrentStatus;
            sendout.CurrentStatus = request.Status;

            // Create status history entry
            var statusHistory = new SendoutStatusHistory
            {
                SendoutId = sendout.Id,
                Status = request.Status,
                Notes = request.Notes ?? $"Status changed from {previousStatus} to {request.Status}.",
                UpdatedByUserId = _currentUserService.UserId ?? 0
            };

            await _unitOfWork.SendoutStatusHistoryRepository.AddAsync(statusHistory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload sendout with related entities for DTO
            var updatedSendout = await _unitOfWork.SendoutRepository.GetByIdAsync(sendout.Id);
            if (updatedSendout == null)
            {
                throw new InvalidOperationException("Failed to retrieve updated sendout.");
            }

            var content = await _unitOfWork.ContentRepository.GetByIdAsync(updatedSendout.ContentId);
            var printer = await _unitOfWork.PrinterRepository.GetByIdAsync(updatedSendout.PrinterId);

            if (content == null || printer == null)
            {
                throw new InvalidOperationException("Failed to retrieve related entities.");
            }

            return MapToDto(updatedSendout, content, printer);
        }

        private bool IsValidStatusTransition(SendoutStatus currentStatus, SendoutStatus newStatus)
        {
            // Define valid status transitions
            return currentStatus switch
            {
                SendoutStatus.Submitted => newStatus == SendoutStatus.Received || 
                                          newStatus == SendoutStatus.Rejected,
                SendoutStatus.Received => newStatus == SendoutStatus.InProduction || 
                                         newStatus == SendoutStatus.Rejected,
                SendoutStatus.InProduction => newStatus == SendoutStatus.Completed || 
                                             newStatus == SendoutStatus.Rejected,
                SendoutStatus.Completed => newStatus == SendoutStatus.Dispatched || 
                                          newStatus == SendoutStatus.Rejected,
                SendoutStatus.Dispatched => false, // Final status, no transitions allowed
                SendoutStatus.Rejected => false, // Final status, no transitions allowed
                _ => false
            };
        }

        private SendoutDto MapToDto(Sendout sendout, Content content, Printer printer)
        {
            return new SendoutDto
            {
                Id = sendout.Id,
                OrganizationId = sendout.OrganizationId,
                OrganizationName = sendout.Organization?.Name ?? string.Empty,
                ContentId = sendout.ContentId,
                ContentName = content.Name,
                PrinterId = sendout.PrinterId,
                PrinterName = printer.Name,
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

