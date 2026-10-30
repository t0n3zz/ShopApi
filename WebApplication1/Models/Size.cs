namespace ShopApi.Models;

public class Size
{
    public int Id { get; set; }
    public string LetterSize { get; set; } = string.Empty;
    public int NumericSize { get; set; }
    public int StockQuantity { get; set; }

    public int ClothingId { get; set; }
    public Clothing Clothing { get; set; } = null!;
}