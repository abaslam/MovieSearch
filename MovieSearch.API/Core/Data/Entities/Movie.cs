using System.Text.RegularExpressions;
using MovieSearch.API.Core.Infrstructure;

namespace MovieSearch.API.Core.Data.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public int Year { get; set; }
        public List<Genre> Genres { get; set; } = new List<Genre>();

        public static Movie Parse(string movieLine)
        {
            Match match = Constants.MovieRecordRegex.Match(movieLine);

            if (match.Success)
            {
                return new Movie
                {
                    Id = int.Parse(match.Groups[1].Value),
                    MovieName = match.Groups[2].Value,
                    Year = int.Parse(match.Groups[3].Value),
                    Genres = match.Groups[4].Value.Split("|", StringSplitOptions.RemoveEmptyEntries).Select(x => new Genre(x)).ToList()
                };
            }

            return null;
        }
    }
}
