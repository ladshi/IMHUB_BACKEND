using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using IMHub.Domain.Enums;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Commands
{
    public class CreateSendoutCommandHandler : IRequestHandler<CreateSendoutCommand, SendoutDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateSendoutCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SendoutDto> Handle(CreateSendoutCommand request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Validate Content exists and belongs to organization
            var content = await _unitOfWork.ContentRepository.GetByIdAsync(request.ContentId);
            if (content == null)
            {
                throw new KeyNotFoundException($"Content with ID {request.ContentId} not found.");
            }

            if (content.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot create sendout for content from another organization.");
            }

            // Validate Content has generated PDF
            if (string.IsNullOrWhiteSpace(content.GeneratedPdfUrl))
            {
                throw new InvalidOperationException("Content must have a generated PDF before creating a sendout.");
            }

            // Validate Printer exists and is linked to organization
            var printer = await _unitOfWork.PrinterRepository.GetByIdAsync(request.PrinterId);
            if (printer == null)
            {
                throw new KeyNotFoundException($"Printer with ID {request.PrinterId} not found.");
            }

            // Check if printer is linked to organization via Distribution
            var distribution = await _unitOfWork.DistributionRepository
                .GetByOrganizationAndPrinterAsync(organizationId, request.PrinterId, cancellationToken);
            
            if (distribution == null)
            {
                throw new InvalidOperationException($"Printer {request.PrinterId} is not linked to your organization.");
            }

            if (!printer.IsActive)
            {
                throw new InvalidOperationException($"Printer {request.PrinterId} is not active.");
            }

            // Generate unique JobReference
            var jobReference = GenerateJobReference(organizationId, request.ContentId);

            // Create Sendout
            var sendout = new Sendout
            {
                OrganizationId = organizationId,
                ContentId = request.ContentId,
                PrinterId = request.PrinterId,
                JobReference = jobReference,
                CurrentStatus = SendoutStatus.Submitted,
                TargetDate = request.TargetDate
            };

            await _unitOfWork.SendoutRepository.AddAsync(sendout);

            // Create initial status history entry
            var statusHistory = new SendoutStatusHistory
            {
                SendoutId = sendout.Id,
                Status = SendoutStatus.Submitted,
                Notes = "Sendout created and submitted.",
                UpdatedByUserId = _currentUserService.UserId ?? 0
            };

            await _unitOfWork.SendoutStatusHistoryRepository.AddAsync(statusHistory);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload sendout with related entities for DTO
            var createdSendout = await _unitOfWork.SendoutRepository.GetByIdAsync(sendout.Id);
            if (createdSendout == null)
            {
                throw new InvalidOperationException("Failed to retrieve created sendout.");
            }

            return MapToDto(createdSendout, content, printer);
        }

        private string GenerateJobReference(int organizationId, int contentId)
        {
            // Format: ORG{orgId}-{timestamp}-{contentId}
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            return $"ORG{organizationId}-{timestamp}-{contentId}";
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

