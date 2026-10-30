namespace ShopApi.Models;

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; } // 1-5
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int ClothingId { get; set; }
    public Clothing Clothing { get; set; } = null!;
}