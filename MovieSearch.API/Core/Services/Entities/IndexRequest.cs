using MediatR;
using Microsoft.AspNetCore.Mvc;
using MovieSearch.API.Core.Infrstructure.Entities;

namespace MovieSearch.API.Core.Services.Entities
{
    public class IndexRequest : IRequest<Response<IndexResponse>>
    {
        [FromBody]
        public IFormFile DataFile { get; set; }

        public bool DeleteIndex { get; set; }
    }
}
