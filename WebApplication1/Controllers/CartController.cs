using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.DTO;
using ShopApi.Models;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ShopDbContext _db;
    public CartController(ShopDbContext db) => _db = db;

    private int GetUserId() => int.Parse(User.FindFirst("sub")!.Value);

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();
        var items = await _db.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Clothing)
            .Select(c => new
            {
                c.Id,
                c.ClothingId,
                Name = c.Clothing.Name,
                Price = c.Clothing.Price,
                c.Quantity,
                Total = c.Clothing.Price * c.Quantity,
                ImagePath = c.Clothing.ImagePath
            })
            .ToListAsync();

        var totalSum = items.Sum(i => i.Total);
        return Ok(new { Items = items, TotalSum = totalSum });
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartRequest request)
    {
        var userId = GetUserId();
        var clothing = await _db.Clothings.FindAsync(request.ClothingId);
        if (clothing == null || !clothing.IsAvailable) return BadRequest("Товар недоступен");

        var existing = await _db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ClothingId == request.ClothingId);

        if (existing != null)
        {
            existing.Quantity += request.Quantity;
        }
        else
        {
            _db.CartItems.Add(new CartItem
            {
                UserId = userId,
                ClothingId = request.ClothingId,
                Quantity = request.Quantity
            });
        }
        await _db.SaveChangesAsync();
        return Ok(new { message = "Добавлено в корзину" });
    }

    [HttpPatch("{cartItemId}")]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, UpdateCartRequest request)
    {
        var userId = GetUserId();
        var item = await _db.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
        if (item == null) return NotFound();

        if (request.Quantity <= 0)
            _db.CartItems.Remove(item);
        else
            item.Quantity = request.Quantity;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Корзина обновлена" });
    }

    [HttpDelete("{cartItemId}")]
    public async Task<IActionResult> Delete(int cartItemId)
    {
        var userId = GetUserId();
        var item = await _db.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
        if (item == null) return NotFound();

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Удалено из корзины" });
    }
}