namespace Pitstop.Models
{
public class ProductMedia
{
    public string? Id { get; set; } // Primary key

    public string? FileName { get; set; }
    public string? Title { get; set; }
    public string? Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

}