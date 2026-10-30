namespace ShopApi.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Created";

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public List<OrderItem> Items { get; set; } = new();

    public decimal TotalPrice { get; set; } // Итоговая стоимость на момент покупки
    public string DeliveryType { get; set; } = "Pickup"; // "Delivery" или "Pickup"
    public string? DeliveryAddress { get; set; } // Если доставка
    public int? PickupPointId { get; set; }
    public PickupPoint? PickupPoint { get; set; } // Если ПВЗ
}