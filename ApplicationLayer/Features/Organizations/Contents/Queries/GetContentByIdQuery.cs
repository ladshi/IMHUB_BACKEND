using MediatR;

namespace IMHub.ApplicationLayer.Features.Organizations.Contents.Queries
{
    public class GetContentByIdQuery : IRequest<ContentWithFieldsDto>
    {
        public int Id { get; set; }
    }
}

