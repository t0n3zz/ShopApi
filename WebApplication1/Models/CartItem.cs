namespace ShopApi.Models;

public class CartItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int ClothingId { get; set; }
    public Clothing Clothing { get; set; } = null!;
}