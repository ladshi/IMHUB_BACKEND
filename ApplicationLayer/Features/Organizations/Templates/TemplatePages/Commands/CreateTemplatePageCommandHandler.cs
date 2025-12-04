using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.Domain.Entities;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplatePages.Commands
{
    public class CreateTemplatePageCommandHandler : IRequestHandler<CreateTemplatePageCommand, TemplatePageDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateTemplatePageCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<TemplatePageDto> Handle(CreateTemplatePageCommand request, CancellationToken cancellationToken)
        {
            // Validate user is Organization Admin
            if (_currentUserService.Role != "OrgAdmin")
            {
                throw new UnauthorizedAccessException("Only Organization Admins can create template pages.");
            }

            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Validate template version exists and belongs to user's organization
            var templateVersion = await _unitOfWork.TemplateVersionRepository.GetByIdAsync(request.TemplateVersionId);
            if (templateVersion == null)
            {
                throw new KeyNotFoundException($"Template version with ID {request.TemplateVersionId} not found.");
            }

            var template = await _unitOfWork.TemplateRepository.GetByIdAsync(templateVersion.TemplateId);
            if (template == null || template.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access template from another organization.");
            }

            // Check if page number already exists for this version
            var existingPage = await _unitOfWork.TemplatePageRepository
                .GetByTemplateVersionIdAndPageNumberAsync(request.TemplateVersionId, request.PageNumber, cancellationToken);
            
            if (existingPage != null)
            {
                throw new InvalidOperationException(
                    $"Page {request.PageNumber} already exists for this template version.");
            }

            var templatePage = new TemplatePage
            {
                TemplateVersionId = request.TemplateVersionId,
                PageNumber = request.PageNumber,
                Width = request.Width,
                Height = request.Height,
                BackgroundImageUrl = request.BackgroundImageUrl
            };

            await _unitOfWork.TemplatePageRepository.AddAsync(templatePage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new TemplatePageDto
            {
                Id = templatePage.Id,
                TemplateVersionId = templatePage.TemplateVersionId,
                PageNumber = templatePage.PageNumber,
                Width = templatePage.Width,
                Height = templatePage.Height,
                BackgroundImageUrl = templatePage.BackgroundImageUrl,
                CreatedAt = templatePage.CreatedAt,
                UpdatedAt = templatePage.UpdatedAt
            };
        }
    }
}

