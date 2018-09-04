namespace MvcMusicStore.Models.Entities
{
    public class Artist
    {
        public virtual int Id { get; set; }
        public virtual int ArtistId { get; set; }
        public virtual string Name { get; set; }
    }
}