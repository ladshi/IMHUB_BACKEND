using MediatR;
using IMHub.ApplicationLayer.Features.Organizations.Contents;

namespace IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Commands
{
    public class GenerateContentFromCsvCommand : IRequest<List<ContentDto>>
    {
        public int CsvUploadId { get; set; }
        public bool GenerateAll { get; set; } = true;
        public List<int>? RowIndices { get; set; } // If GenerateAll = false, specify which rows to generate
    }
}

