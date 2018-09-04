using System.ComponentModel.DataAnnotations;

namespace MvcMusicStore.Domain.Entities
{
    public class Album
    {
        public Album(int albumId, string title, decimal price, string albumArtUrl, Genre genre, Artist artist)
        {
            AlbumId = albumId;
            Title = title;
            Price = price;
            AlbumArtUrl = albumArtUrl;
            Genre = genre;
            Artist = artist;
        }

        public int AlbumId { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string Title { get; set; }

        [Range(0.01, 100.00)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string AlbumArtUrl { get; set; }
        public virtual Genre Genre { get; set; }
        public virtual Artist Artist { get; set; }
    }
}