using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateVersions.Commands
{
    public class SetActiveVersionCommandHandler : IRequestHandler<SetActiveVersionCommand, TemplateVersionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public SetActiveVersionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<TemplateVersionDto> Handle(SetActiveVersionCommand request, CancellationToken cancellationToken)
        {
            // Validate user is Organization Admin
            if (_currentUserService.Role != "OrgAdmin")
            {
                throw new UnauthorizedAccessException("Only Organization Admins can set active version.");
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

            // Get the version to activate
            var versionToActivate = await _unitOfWork.TemplateVersionRepository.GetByIdAsync(request.VersionId);
            if (versionToActivate == null)
            {
                throw new KeyNotFoundException($"Template version with ID {request.VersionId} not found.");
            }

            if (versionToActivate.TemplateId != request.TemplateId)
            {
                throw new InvalidOperationException("Version does not belong to the specified template.");
            }

            // Deactivate all other versions
            var allVersions = await _unitOfWork.TemplateVersionRepository
                .GetByTemplateIdAsync(request.TemplateId, cancellationToken);
            
            foreach (var version in allVersions)
            {
                version.IsActive = false;
                await _unitOfWork.TemplateVersionRepository.UpdateAsync(version);
            }

            // Activate the selected version
            versionToActivate.IsActive = true;
            await _unitOfWork.TemplateVersionRepository.UpdateAsync(versionToActivate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new TemplateVersionDto
            {
                Id = versionToActivate.Id,
                TemplateId = versionToActivate.TemplateId,
                VersionNumber = versionToActivate.VersionNumber,
                PdfUrl = versionToActivate.PdfUrl,
                DesignJson = versionToActivate.DesignJson,
                IsActive = versionToActivate.IsActive,
                CreatedAt = versionToActivate.CreatedAt,
                UpdatedAt = versionToActivate.UpdatedAt
            };
        }
    }
}

