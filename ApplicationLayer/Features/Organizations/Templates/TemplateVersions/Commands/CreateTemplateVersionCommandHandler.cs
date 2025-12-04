using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateVersions.Commands
{
    public class CreateTemplateVersionCommandHandler : IRequestHandler<CreateTemplateVersionCommand, TemplateVersionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateTemplateVersionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<TemplateVersionDto> Handle(CreateTemplateVersionCommand request, CancellationToken cancellationToken)
        {
            // Validate user is Organization Admin
            if (_currentUserService.Role != "OrgAdmin")
            {
                throw new UnauthorizedAccessException("Only Organization Admins can create template versions.");
            }

            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Validate template exists and belongs to user's organization
            var template = await _unitOfWork.TemplateRepository.GetByIdAsync(request.TemplateId);
            if (template == null)
            {
                throw new KeyNotFoundException($"Template with ID {request.TemplateId} not found.");
            }

            if (template.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access template from another organization.");
            }

            // Check if version number already exists for this template
            var existingVersion = await _unitOfWork.TemplateVersionRepository
                .GetByTemplateIdAndVersionNumberAsync(request.TemplateId, request.VersionNumber, cancellationToken);
            
            if (existingVersion != null)
            {
                throw new InvalidOperationException(
                    $"Version {request.VersionNumber} already exists for this template.");
            }

            // If setting as active, deactivate other versions
            if (request.IsActive)
            {
                var activeVersion = await _unitOfWork.TemplateVersionRepository
                    .GetActiveVersionByTemplateIdAsync(request.TemplateId, cancellationToken);
                
                if (activeVersion != null)
                {
                    activeVersion.IsActive = false;
                    await _unitOfWork.TemplateVersionRepository.UpdateAsync(activeVersion);
                }
            }

            var templateVersion = new TemplateVersion
            {
                TemplateId = request.TemplateId,
                VersionNumber = request.VersionNumber,
                PdfUrl = request.PdfUrl,
                DesignJson = request.DesignJson,
                IsActive = request.IsActive
            };

            await _unitOfWork.TemplateVersionRepository.AddAsync(templateVersion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new TemplateVersionDto
            {
                Id = templateVersion.Id,
                TemplateId = templateVersion.TemplateId,
                VersionNumber = templateVersion.VersionNumber,
                PdfUrl = templateVersion.PdfUrl,
                DesignJson = templateVersion.DesignJson,
                IsActive = templateVersion.IsActive,
                CreatedAt = templateVersion.CreatedAt,
                UpdatedAt = templateVersion.UpdatedAt
            };
        }
    }
}

