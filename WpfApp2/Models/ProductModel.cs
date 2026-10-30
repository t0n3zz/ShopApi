namespace Marketplace.App.Models;

public class ProductModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public bool IsAvailable { get; set; }
    public double AverageRating { get; set; }
    public int CategoryId { get; set; }
}