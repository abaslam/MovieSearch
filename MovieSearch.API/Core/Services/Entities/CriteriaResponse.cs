namespace MovieSearch.API.Core.Services.Entities
{
    public class CriteriaResponse
    {
        public List<int> Years { get; set; } = new List<int>();
        public List<string> Genres { get; set; } = new List<string>();
    }
}
