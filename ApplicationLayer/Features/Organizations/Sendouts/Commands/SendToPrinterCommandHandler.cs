using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Features.Organizations.Sendouts;
using IMHub.Domain.Entities;
using IMHub.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Commands
{
    public class SendToPrinterCommandHandler : IRequestHandler<SendToPrinterCommand, SendoutDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SendToPrinterCommandHandler> _logger;

        public SendToPrinterCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IHttpClientFactory httpClientFactory,
            ILogger<SendToPrinterCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<SendoutDto> Handle(SendToPrinterCommand request, CancellationToken cancellationToken)
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
                throw new UnauthorizedAccessException("Cannot access sendout from another organization.");
            }

            // Validate sendout is in Submitted status
            if (sendout.CurrentStatus != SendoutStatus.Submitted)
            {
                throw new InvalidOperationException($"Cannot send sendout. Current status is {sendout.CurrentStatus}. Only 'Submitted' sendouts can be sent.");
            }

            // Get content with PDF URL
            var content = await _unitOfWork.ContentRepository.GetByIdAsync(sendout.ContentId);
            if (content == null)
            {
                throw new KeyNotFoundException($"Content with ID {sendout.ContentId} not found.");
            }

            if (string.IsNullOrWhiteSpace(content.GeneratedPdfUrl))
            {
                throw new InvalidOperationException("Content must have a generated PDF URL.");
            }

            // Get printer with API details
            var printer = await _unitOfWork.PrinterRepository.GetByIdAsync(sendout.PrinterId);
            if (printer == null)
            {
                throw new KeyNotFoundException($"Printer with ID {sendout.PrinterId} not found.");
            }

            if (string.IsNullOrWhiteSpace(printer.ApiKey))
            {
                throw new InvalidOperationException("Printer API key is not configured.");
            }

            // Call printer API
            try
            {
                await SendToPrinterApiAsync(sendout, content, printer, cancellationToken);

                // Update sendout status to Received
                sendout.CurrentStatus = SendoutStatus.Received;
                
                // Create status history entry
                var statusHistory = new SendoutStatusHistory
                {
                    SendoutId = sendout.Id,
                    Status = SendoutStatus.Received,
                    Notes = "Successfully sent to printer API.",
                    UpdatedByUserId = _currentUserService.UserId ?? 0
                };

                await _unitOfWork.SendoutStatusHistoryRepository.AddAsync(statusHistory);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Sendout {SendoutId} successfully sent to printer {PrinterId}", 
                    sendout.Id, printer.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send sendout {SendoutId} to printer {PrinterId}", 
                    sendout.Id, printer.Id);

                // Update status to Rejected on API failure
                sendout.CurrentStatus = SendoutStatus.Rejected;
                
                var statusHistory = new SendoutStatusHistory
                {
                    SendoutId = sendout.Id,
                    Status = SendoutStatus.Rejected,
                    Notes = $"Failed to send to printer API: {ex.Message}",
                    UpdatedByUserId = _currentUserService.UserId ?? 0
                };

                await _unitOfWork.SendoutStatusHistoryRepository.AddAsync(statusHistory);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new InvalidOperationException($"Failed to send to printer API: {ex.Message}", ex);
            }

            // Reload sendout for DTO
            var updatedSendout = await _unitOfWork.SendoutRepository.GetByIdAsync(sendout.Id);
            if (updatedSendout == null)
            {
                throw new InvalidOperationException("Failed to retrieve updated sendout.");
            }

            return MapToDto(updatedSendout, content, printer);
        }

        private async Task SendToPrinterApiAsync(Sendout sendout, Content content, Printer printer, CancellationToken cancellationToken)
        {
            // Note: This is a placeholder implementation. 
            // Replace with actual printer API endpoint and payload structure.
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X-API-Key", printer.ApiKey);
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var payload = new
            {
                jobReference = sendout.JobReference,
                pdfUrl = content.GeneratedPdfUrl,
                targetDate = sendout.TargetDate,
                organizationId = sendout.OrganizationId,
                contentId = content.Id
            };

            // TODO: Replace with actual printer API endpoint
            var apiEndpoint = $"https://printer-api.example.com/jobs"; // Replace with actual endpoint
            
            var response = await httpClient.PostAsJsonAsync(apiEndpoint, payload, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Printer API returned {response.StatusCode}: {errorContent}");
            }
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

