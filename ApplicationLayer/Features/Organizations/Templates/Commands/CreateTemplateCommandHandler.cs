using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.Commands
{
    public class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, TemplateDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateTemplateCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<TemplateDto> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
        {
            // Validate user is Organization Admin
            if (_currentUserService.Role != "OrgAdmin")
            {
                throw new UnauthorizedAccessException("Only Organization Admins can create templates.");
            }

            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Check if slug already exists
            if (await _unitOfWork.TemplateRepository.ExistsBySlugAsync(request.Slug, cancellationToken))
            {
                throw new InvalidOperationException($"Template with slug '{request.Slug}' already exists.");
            }

            var template = new Template
            {
                OrganizationId = organizationId,
                Title = request.Title,
                Slug = request.Slug,
                ThumbnailUrl = request.ThumbnailUrl,
                Status = request.Status,
                MetadataJson = request.MetadataJson
            };

            await _unitOfWork.TemplateRepository.AddAsync(template);
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

