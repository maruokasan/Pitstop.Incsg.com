namespace Pitstop.Models
{
    public class FeaturedItem
    {
        public string? Id { get; set; }
        public string? FileName { get; set; }
        public string? Title { get; set; }
        public string? Price { get; set; }
        public string? Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}