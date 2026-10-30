using Marketplace.App.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Marketplace.App.Services;

public class ApiService
{
    private readonly HttpClient _http;
    public string? Token { get; set; }
    public string CurrentLogin { get; private set; } = "";
    public string CurrentRole { get; private set; } = "";
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
    public string BaseUrl => "http://localhost:5179";

    public ApiService()
    {
        _http = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    public async Task<bool> LoginAsync(string login, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new { login, password });
            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content)) return false;

            var result = JsonSerializer.Deserialize<LoginResponse>(content);
            if (result == null || string.IsNullOrWhiteSpace(result.AccessToken)) return false;

            Token = result.AccessToken;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var parts = Token.Split('.');
            if (parts.Length < 2) return false;

            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            var claims = JsonDocument.Parse(json).RootElement;

            CurrentLogin = claims.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";
            CurrentRole = claims.TryGetProperty("role", out var roleProp) ? roleProp.GetString() ?? "" : "";

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool success, string message)> RegisterAsync(string login, string password)
    {
        try
        {
            var request = new { login, password, phone = "000", address = "Не указан" };
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            var content = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, content);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public void Logout()
    {
        Token = null;
        _http.DefaultRequestHeaders.Authorization = null;
        CurrentLogin = "";
        CurrentRole = "";
    }

    public async Task<List<ProductModel>> GetProductsAsync(string? search = null, int? categoryId = null)
    {
        try
        {
            var query = "api/clothing?";
            if (!string.IsNullOrEmpty(search)) query += $"search={Uri.EscapeDataString(search)}&";
            if (categoryId.HasValue) query += $"categoryId={categoryId}&";

            var list = await _http.GetFromJsonAsync<List<ProductModel>>(query) ?? new();

            foreach (var p in list)
                if (!string.IsNullOrEmpty(p.ImagePath))
                    p.ImagePath = BaseUrl + p.ImagePath;

            return list;
        }
        catch
        {
            return new List<ProductModel>();
        }
    }

    public async Task<List<CategoryModel>> GetCategoriesAsync()
    {
        try { return await _http.GetFromJsonAsync<List<CategoryModel>>("api/categories") ?? new(); }
        catch { return new List<CategoryModel>(); }
    }

    public async Task<CartResponse?> GetCartAsync()
    {
        try
        {
            var cart = await _http.GetFromJsonAsync<CartResponse>("api/cart");
            if (cart != null)
                foreach (var item in cart.Items)
                    if (!string.IsNullOrEmpty(item.ImagePath))
                        item.ImagePath = BaseUrl + item.ImagePath;
            return cart;
        }
        catch { return null; }
    }

    public async Task<bool> AddToCartAsync(int clothingId, int quantity)
    {
        try { return (await _http.PostAsJsonAsync("api/cart", new { clothingId, quantity })).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task RemoveCartItemAsync(int cartItemId)
    {
        try { await _http.DeleteAsync($"api/cart/{cartItemId}"); } catch { }
    }
}