using System;
using System.ComponentModel.DataAnnotations;

namespace MvcMusicStore.Domain.Entities
{
    public class Cart
    {
        public int Quantity { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        public virtual Album Album { get; set; }
    }
}