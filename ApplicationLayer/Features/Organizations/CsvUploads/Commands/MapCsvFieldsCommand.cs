using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Commands
{
    public class MapCsvFieldsCommand : IRequest<CsvUploadDto>
    {
        public int CsvUploadId { get; set; }
        public Dictionary<string, string> Mappings { get; set; } = new(); // CSV Column -> Template Field Name
    }
}

