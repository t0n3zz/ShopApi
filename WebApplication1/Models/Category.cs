namespace ShopApi.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Clothing> Clothings { get; set; } = new();
}