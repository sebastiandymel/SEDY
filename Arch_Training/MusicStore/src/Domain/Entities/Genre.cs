namespace MvcMusicStore.Domain.Entities
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Genre(int genreId, string name, string description)
        {
            GenreId = genreId;
            Name = name;
            Description = description;
        }
    }
}