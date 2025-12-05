using MediatR;
using Microsoft.AspNetCore.Http;
using IMHub.ApplicationLayer.Features.Organizations.CsvUploads;

namespace IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Commands
{
    public class UploadCsvCommand : IRequest<CsvUploadDto>
    {
        public int TemplateId { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}

