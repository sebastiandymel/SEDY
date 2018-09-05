using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MvcMusicStore.Models
{
    public class CheckoutDetailsViewModel
    {
        [ScaffoldColumn(true)]
        public int OrderId { get; set; }

        [ScaffoldColumn(true)]
        public DateTime OrderDate { get; set; }

        [ScaffoldColumn(true)]
        public string Username { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }

        public string Country { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [DisplayName("Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [ScaffoldColumn(true)]
        public decimal Total { get; set; }

        public List<CheckoutOrderDetailVM> OrderDetails { get; set; }
    }

    public class CheckoutOrderDetailVM
    {
        [DisplayName("Album name")]
        public string AlbumName { get; set; }

        [DisplayName("Album price")]
        public decimal UnitPrice { get; set; }

        [DisplayName("Email Address")]
        public int Quantity { get; set; }
    }
}