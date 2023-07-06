using System.Text.RegularExpressions;

namespace MovieSearch.API.Core.Data.Entities
{
    public class Movie
    {
        private static Regex regex = new Regex(@"^(\d+)::(.*?) \((\d+)\)::(.+)$", RegexOptions.Compiled);
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public List<string> Genres { get; set; } = new List<string>();

        public static Movie Parse(string movieLine)
        {
            Match match = regex.Match(movieLine);

            if(match.Success)
            {
                return new Movie
                {
                    Id = int.Parse(match.Groups[1].Value),
                    Name = match.Groups[2].Value,
                    Year = int.Parse(match.Groups[3].Value),
                    Genres = match.Groups[4].Value.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList()
                };
            }

            return null;
        }
    }
}
