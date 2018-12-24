namespace MvcMusicStore.Domain.Entities
{
    public class Artist
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }

        public Artist(int artistId, string name)
        {
            ArtistId = artistId;
            Name = name;
        }
    }
}