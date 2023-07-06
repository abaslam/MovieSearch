using System.Text.RegularExpressions;

namespace MovieSearch.API.Core.Infrstructure
{
    public class Constants
    {
        public const string MovieDataIndexName = "movie_data";
        
        public static Regex MovieRecordRegex = new Regex(@"^(\d+)::(.*?) \((\d+)\)::(.+)$", RegexOptions.Compiled);
    }
}
