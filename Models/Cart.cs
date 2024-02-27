namespace Pitstop.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string? SessionId { get; set; }
        public string? UserId { get; set; }
        public string? ProductId { get; set; }
        public string? Quantity { get; set; }
        public string? Size { get; set; }
        public DateTime CreateAt { get; set; }

    }
}