using System.Collections.Generic;

namespace MvcMusicStore.Models.Entities
{
    public class Genre
    {
        public virtual int Id { get; set; }
        public virtual int GenreId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual List<Album> Albums { get; set; }
    }
}