namespace ShopApi.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ClothingId { get; set; }
    public Clothing Clothing { get; set; } = null!;

    public int SizeId { get; set; }
    public Size Size { get; set; } = null!;

    public decimal PriceAtPurchase { get; set; } // Цена на момент покупки
}