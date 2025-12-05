using IMHub.ApplicationLayer.Common.Interfaces;
using IMHub.ApplicationLayer.Common.Interfaces.IRepositories;
using IMHub.ApplicationLayer.Features.Organizations.Contents;
using IMHub.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Text.Json;

namespace IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Commands
{
    public class GenerateContentFromCsvCommandHandler : IRequestHandler<GenerateContentFromCsvCommand, List<ContentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHostEnvironment _hostEnvironment;

        public GenerateContentFromCsvCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<List<ContentDto>> Handle(GenerateContentFromCsvCommand request, CancellationToken cancellationToken)
        {
            // Validate user is Organization Admin or Employee
            if (_currentUserService.Role != "OrgAdmin" && _currentUserService.Role != "Employee")
            {
                throw new UnauthorizedAccessException("Only Organization Admins and Employees can generate content from CSV.");
            }

            // Get current user's organization ID
            if (!_currentUserService.OrganizationId.HasValue)
            {
                throw new UnauthorizedAccessException("Organization ID not found in user context.");
            }

            var organizationId = _currentUserService.OrganizationId.Value;

            var csvUpload = await _unitOfWork.CsvUploadRepository.GetByIdAsync(request.CsvUploadId);
            if (csvUpload == null)
            {
                throw new KeyNotFoundException($"CSV upload with ID {request.CsvUploadId} not found.");
            }

            // Ensure CSV upload belongs to user's organization
            if (csvUpload.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("Cannot access CSV upload from another organization.");
            }

            // Validate mapping exists
            if (string.IsNullOrWhiteSpace(csvUpload.MappingJson) || csvUpload.MappingJson == "{}")
            {
                throw new InvalidOperationException("CSV fields must be mapped before generating content.");
            }

            var mappings = JsonSerializer.Deserialize<Dictionary<string, string>>(csvUpload.MappingJson)
                ?? throw new InvalidOperationException("Invalid mapping JSON.");

            // Get active template version
            var activeVersion = await _unitOfWork.TemplateVersionRepository
                .GetActiveVersionByTemplateIdAsync(csvUpload.TemplateId, cancellationToken);
            
            if (activeVersion == null)
            {
                throw new InvalidOperationException("No active template version found.");
            }

            // Get all template fields
            var pages = await _unitOfWork.TemplatePageRepository
                .GetByTemplateVersionIdAsync(activeVersion.Id, cancellationToken);

            var allFields = new List<TemplateField>();
            foreach (var page in pages)
            {
                var fields = await _unitOfWork.TemplateFieldRepository
                    .GetByTemplatePageIdAsync(page.Id, cancellationToken);
                allFields.AddRange(fields);
            }

            var fieldDict = allFields.ToDictionary(f => f.FieldName);

            // Read CSV file
            var csvData = await ReadCsvFileAsync(csvUpload.FileUrl, cancellationToken);
            
            if (csvData.Count == 0)
            {
                throw new InvalidOperationException("CSV file is empty or has no data rows.");
            }

            // Determine which rows to process
            var rowsToProcess = request.GenerateAll
                ? Enumerable.Range(0, csvData.Count).ToList()
                : request.RowIndices ?? new List<int>();

            var generatedContents = new List<ContentDto>();

            foreach (var rowIndex in rowsToProcess)
            {
                if (rowIndex < 0 || rowIndex >= csvData.Count)
                {
                    continue; // Skip invalid indices
                }

                var row = csvData[rowIndex];
                var rowData = row;

                // Create Content instance
                var content = new Content
                {
                    OrganizationId = organizationId,
                    TemplateVersionId = activeVersion.Id,
                    CsvUploadId = csvUpload.Id,
                    Name = $"Content from CSV Row {rowIndex + 1}",
                    Status = "Draft"
                };

                await _unitOfWork.ContentRepository.AddAsync(content);
                await _unitOfWork.SaveChangesAsync(cancellationToken); // Save to get Content ID

                // Create ContentFieldValue for each mapped field
                foreach (var mapping in mappings)
                {
                    var csvColumn = mapping.Key;
                    var templateFieldName = mapping.Value;

                    if (!rowData.ContainsKey(csvColumn))
                    {
                        continue; // Skip if CSV column doesn't exist
                    }

                    if (!fieldDict.ContainsKey(templateFieldName))
                    {
                        continue; // Skip if template field doesn't exist
                    }

                    var templateField = fieldDict[templateFieldName];

                    // Skip locked fields (Employee cannot edit these, so don't populate from CSV)
                    // But OrgAdmin can still populate them
                    if (templateField.IsLocked && _currentUserService.Role == "Employee")
                    {
                        continue;
                    }

                    var fieldValue = new ContentFieldValue
                    {
                        ContentId = content.Id,
                        TemplateFieldId = templateField.Id,
                        Value = rowData[csvColumn] ?? string.Empty
                    };

                    await _unitOfWork.ContentFieldValueRepository.AddAsync(fieldValue);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                generatedContents.Add(new ContentDto
                {
                    Id = content.Id,
                    OrganizationId = content.OrganizationId,
                    TemplateVersionId = content.TemplateVersionId,
                    CsvUploadId = content.CsvUploadId,
                    Name = content.Name,
                    Status = content.Status,
                    GeneratedPdfUrl = content.GeneratedPdfUrl,
                    CreatedAt = content.CreatedAt,
                    UpdatedAt = content.UpdatedAt
                });
            }

            return generatedContents;
        }

        private async Task<List<Dictionary<string, string>>> ReadCsvFileAsync(string fileUrl, CancellationToken cancellationToken)
        {
            var csvData = new List<Dictionary<string, string>>();

            // Convert relative URL to full file path
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var filePath = Path.Combine(webRootPath, fileUrl.TrimStart('/'));

            // Read CSV file from local storage
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CSV file not found at path: {filePath}");
            }

            var csvContent = await File.ReadAllTextAsync(filePath, cancellationToken);
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 2)
            {
                return csvData; // No data rows
            }

            // First line is header
            var headers = lines[0].Split(',').Select(h => h.Trim().Trim('"')).ToArray();

            // Process data rows
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',').Select(v => v.Trim().Trim('"')).ToArray();
                
                if (values.Length != headers.Length)
                {
                    continue; // Skip malformed rows
                }

                var row = new Dictionary<string, string>();
                for (int j = 0; j < headers.Length; j++)
                {
                    row[headers[j]] = values[j];
                }

                csvData.Add(row);
            }

            return csvData;
        }
    }
}

