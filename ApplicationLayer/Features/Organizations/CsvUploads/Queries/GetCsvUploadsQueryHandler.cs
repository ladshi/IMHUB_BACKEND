using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Common.Models;
using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Queries
{
    public class GetCsvUploadsQueryHandler : IRequestHandler<GetCsvUploadsQuery, PagedResult<CsvUploadDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetCsvUploadsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<CsvUploadDto>> Handle(GetCsvUploadsQuery request, CancellationToken cancellationToken)
        {
            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            // Get CSV uploads for user's organization only
            var csvUploads = await _unitOfWork.CsvUploadRepository
                .GetByOrganizationIdAsync(organizationId, cancellationToken);

            // Apply TemplateId filter if provided
            if (request.TemplateId.HasValue)
            {
                csvUploads = csvUploads.Where(c => c.TemplateId == request.TemplateId.Value).ToList();
            }

            var totalCount = csvUploads.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Load templates for DTO
            var templates = await _unitOfWork.TemplateRepository.GetAllAsync();
            var templateDict = templates.ToDictionary(t => t.Id);

            // Apply pagination
            var pagedCsvUploads = csvUploads
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CsvUploadDto
                {
                    Id = c.Id,
                    OrganizationId = c.OrganizationId,
                    TemplateId = c.TemplateId,
                    TemplateName = templateDict.ContainsKey(c.TemplateId) ? templateDict[c.TemplateId].Title : string.Empty,
                    FileName = c.FileName,
                    FileUrl = c.FileUrl,
                    TotalRows = c.TotalRows,
                    MappingJson = c.MappingJson,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToList();

            return new PagedResult<CsvUploadDto>
            {
                Items = pagedCsvUploads,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}

