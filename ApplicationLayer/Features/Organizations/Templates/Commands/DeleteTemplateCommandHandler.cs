using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.Commands
{
    public class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteTemplateCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
        {
            // Validate user is Organization Admin
            if (_currentUserService.Role != "OrgAdmin")
            {
                throw new UnauthorizedAccessException("Only Organization Admins can delete templates.");
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

            // Soft delete
            await _unitOfWork.TemplateRepository.DeleteAsync(template);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

