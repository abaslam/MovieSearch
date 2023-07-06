namespace MovieSearch.API.Core.Data.Entities
{
    public class Genre
    {
        public Genre(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
