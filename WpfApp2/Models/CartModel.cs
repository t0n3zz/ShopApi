using System.Text.Json.Serialization;

namespace Marketplace.App.Models;

public class CartItemModel
{
    public int Id { get; set; }
    public int ClothingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public string? ImagePath { get; set; }
}

public class CartResponse
{
    public List<CartItemModel> Items { get; set; } = new();
    public decimal TotalSum { get; set; }
}

public class LoginResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = "";
}