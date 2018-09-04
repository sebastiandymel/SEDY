using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcMusicStore.Models.Entities
{
    [Bind(Include = "FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email")]
    public class Order
    {
        [ScaffoldColumn(false)]
        public virtual int Id { get; set; }
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }

        [ScaffoldColumn(false)]
        public virtual DateTime OrderDate { get; set; }

        [ScaffoldColumn(false)]
        public virtual string Username { get; set; }

        [Required]
        [DisplayName("First Name")]
        [StringLength(160)]
        public virtual string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(160)]
        public virtual string LastName { get; set; }

        [Required]
        [StringLength(70, MinimumLength = 3)]
        public virtual string Address { get; set; }

        [Required]
        [StringLength(40)]
        public virtual string City { get; set; }

        [Required]
        [StringLength(40)]
        public virtual string State { get; set; }

        [Required]
        [DisplayName("Postal Code")]
        [StringLength(10, MinimumLength = 5)]
        public virtual string PostalCode { get; set; }

        [Required]
        [StringLength(40)]
        public virtual string Country { get; set; }

        [Required]
        [StringLength(24)]
        [DataType(DataType.PhoneNumber)]
        public virtual string Phone { get; set; }

        [Required]
        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public virtual string Email { get; set; }

        [ScaffoldColumn(false)]
        public virtual decimal Total { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }
    }

    public enum OrderStatus
    {
        New,
        Collecting,
        Collcted,
        PreparingPackage,
        Shipped
    }
}