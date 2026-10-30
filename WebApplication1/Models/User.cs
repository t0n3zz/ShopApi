namespace ShopApi.Models;

public class User
{
    public int Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";

    public List<Order> Orders { get; set; } = new();
    public List<CartItem> CartItems { get; set; } = new();
    public List<Favorite> Favorites { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}