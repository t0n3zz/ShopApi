namespace ShopApi.Models;

public class PickupPoint
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}