using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly ShopDbContext _db;
    public FavoritesController(ShopDbContext db) => _db = db;

    private int GetUserId() => int.Parse(User.FindFirst("sub")!.Value);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var favorites = await _db.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Clothing)
            .Select(f => new { f.Id, f.ClothingId, f.Clothing.Name, f.Clothing.Price })
            .ToListAsync();
        return Ok(favorites);
    }

    [HttpPost("{clothingId}")]
    public async Task<IActionResult> Add(int clothingId)
    {
        var userId = GetUserId();
        if (await _db.Favorites.AnyAsync(f => f.UserId == userId && f.ClothingId == clothingId))
            return BadRequest("Уже в избранном");

        _db.Favorites.Add(new Favorite { UserId = userId, ClothingId = clothingId });
        await _db.SaveChangesAsync();
        return Ok(new { message = "Добавлено в избранное" });
    }

    [HttpDelete("{clothingId}")]
    public async Task<IActionResult> Remove(int clothingId)
    {
        var userId = GetUserId();
        var fav = await _db.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ClothingId == clothingId);
        if (fav == null) return NotFound();

        _db.Favorites.Remove(fav);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Удалено из избранного" });
    }
}