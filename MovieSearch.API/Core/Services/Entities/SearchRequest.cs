using MediatR;
using MovieSearch.API.Core.Infrstructure.Entities;

namespace MovieSearch.API.Core.Services.Entities
{
    public class SearchRequest : IRequest<Response<SearchResponse>>
    {
        public int PageSize { get; set; }

        public List<int> Years { get; set; } = new List<int>();
        public List<string> Genres { get; set; } = new List<string>();
        public string SearchTerm { get; set; }
    }
}
