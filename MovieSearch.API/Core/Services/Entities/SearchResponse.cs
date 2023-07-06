using MovieSearch.API.Core.Data.Entities;

namespace MovieSearch.API.Core.Services.Entities
{
    public class SearchResponse
    {
        public long TotalCount { get; set; }
        public List<Movie> Movies { get; set; }
        public List<KeyValuePair<int, long>> Years { get; set; }
        public List<KeyValuePair<string, long>> Genres { get; set; }
    }
}
