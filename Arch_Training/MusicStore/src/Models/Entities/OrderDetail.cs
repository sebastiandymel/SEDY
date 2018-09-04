using System.ComponentModel.DataAnnotations;

namespace MvcMusicStore.Models.Entities
{
    public class OrderDetail
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal UnitPrice { get; set; }
        /// <summary>
        /// Id dostawy od dystrybutora
        /// </summary>
        public virtual string ShipmentId { get; set; }

        public virtual Album Album { get; set; }
        public virtual Order Order { get; set; }

        public virtual int OrderId { get; set; }
        public virtual int AlbumId { get; set; }
    }
}