using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Common.Models;
using IMHub.Domain.Enums;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Queries
{
    public class GetSendoutsQueryHandler : IRequestHandler<GetSendoutsQuery, PagedResult<SendoutDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetSendoutsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<SendoutDto>> Handle(GetSendoutsQuery request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Get sendouts for user's organization only
            var sendouts = await _unitOfWork.SendoutRepository
                .GetByOrganizationIdAsync(organizationId, cancellationToken);

            // Apply filters
            if (request.Status.HasValue)
            {
                sendouts = sendouts.Where(s => s.CurrentStatus == request.Status.Value).ToList();
            }

            if (request.ContentId.HasValue)
            {
                sendouts = sendouts.Where(s => s.ContentId == request.ContentId.Value).ToList();
            }

            if (request.PrinterId.HasValue)
            {
                sendouts = sendouts.Where(s => s.PrinterId == request.PrinterId.Value).ToList();
            }

            var totalCount = sendouts.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Apply pagination
            var pagedSendouts = sendouts
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var sendoutDtos = pagedSendouts.Select(s => MapToDto(s)).ToList();

            return new PagedResult<SendoutDto>
            {
                Items = sendoutDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages
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

