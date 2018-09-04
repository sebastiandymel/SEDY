using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MvcMusicStore.Models.Entities
{
    public class Album
    {
        public virtual int Id { get; set; }
        [ScaffoldColumn(false)]

        public virtual int GenreId { get; set; }

        public virtual int ArtistId { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        public virtual string Title { get; set; }

        [Required]
        [Range(0.01, 100.00)]
        [DataType(DataType.Currency)]
        public virtual decimal Price { get; set; }

        [DisplayName("Album Art URL")]
        [StringLength(1024)]
        public virtual string AlbumArtUrl { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
    }
}