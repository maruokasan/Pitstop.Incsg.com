using System.ComponentModel.DataAnnotations.Schema;

namespace Pitstop.Models
{
    public class Product
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? SKU { get; set; }
        public string? Qty { get; set; }
        public string? Price { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
        public string? Thumbnail { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public string? Media { get; set; }
        public string? Discount { get; set; }
        public string? Tax { get; set; }
        public string? Vat { get; set; }
        public string? Barcode { get; set; }
        public string? Size { get; set; }
        public DateTime? CreatedDate { get; set; }

        // Add AvailableSizes property to hold individual sizes
        [NotMapped]
        public List<string> AvailableSizes { get; set; }
    }
}
