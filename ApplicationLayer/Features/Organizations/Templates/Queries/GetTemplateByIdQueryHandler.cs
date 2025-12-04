using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.Queries
{
    public class GetTemplateByIdQueryHandler : IRequestHandler<GetTemplateByIdQuery, TemplateDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetTemplateByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<TemplateDto> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
        {
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

