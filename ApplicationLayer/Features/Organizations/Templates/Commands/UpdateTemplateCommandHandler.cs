using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.Commands
{
    public class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, TemplateDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateTemplateCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<TemplateDto> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
        {
            // Validate user is Organization Admin
            if (_currentUserService.Role != "OrgAdmin")
            {
                throw new UnauthorizedAccessException("Only Organization Admins can update templates.");
            }

            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            var template = await _unitOfWork.TemplateRepository.GetByIdAsync(request.Id);
            if (template == null)
            {
                throw new KeyNotFoundException($"Template with ID {request.Id} not found.");
            }

            // Ensure template belongs to user's organization (multi-tenant isolation)
            if (template.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access template from another organization.");
            }

            // Check if slug is being changed and if new slug already exists
            if (template.Slug != request.Slug)
            {
                if (await _unitOfWork.TemplateRepository.ExistsBySlugAsync(request.Slug, cancellationToken))
                {
                    throw new InvalidOperationException($"Template with slug '{request.Slug}' already exists.");
                }
            }

            template.Title = request.Title;
            template.Slug = request.Slug;
            template.ThumbnailUrl = request.ThumbnailUrl;
            template.Status = request.Status;
            template.MetadataJson = request.MetadataJson;

            await _unitOfWork.TemplateRepository.UpdateAsync(template);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new TemplateDto
            {
                Id = template.Id,
                OrganizationId = template.OrganizationId,
                Title = template.Title,
                Slug = template.Slug,
                ThumbnailUrl = template.ThumbnailUrl,
                Status = template.Status.ToString(),
                MetadataJson = template.MetadataJson,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt
            };
        }
    }
}

