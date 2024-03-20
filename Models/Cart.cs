using System;
using System.ComponentModel.DataAnnotations;
using Pitstop.Models.PitstopData;
using System.ComponentModel.DataAnnotations.Schema;


namespace Pitstop.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public string SessionId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string Size { get; set; }

        public decimal TotalPrice { get; set; } // Calculated based on product price and quantity

        public decimal Discount { get; set; } // If applicable

        public decimal Tax { get; set; } // If applicable

        public string ShippingAddress { get; set; } // If applicable

        public string ShippingMethod { get; set; } // If applicable

        public decimal ShippingCost { get; set; } // If applicable

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation property for associated user
        public User User { get; set; }
        public Product Product { get; set; }

        [NotMapped]
        public List<string> AvailableSizes { get; set; }
    }
}
