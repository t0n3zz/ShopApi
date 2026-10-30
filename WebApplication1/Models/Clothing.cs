namespace ShopApi.Models;

public class Clothing
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public int PurchaseCount { get; set; } = 0;
    public string Type { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;

    public List<Size> Sizes { get; set; } = new();
    public List<OrderItem> OrderItems { get; set; } = new();

    public string? ImagePath { get; set; } // Путь к картинке
    public double AverageRating { get; set; } // Средняя оценка
    public int? PackQuantity { get; set; } // Штук в упаковке (если есть)

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public List<Review> Reviews { get; set; } = new();
    public List<Favorite> Favorites { get; set; } = new();
    public List<CartItem> CartItems { get; set; } = new();
}