using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.DTO;
using ShopApi.Models;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly ShopDbContext _db;
    public ReviewsController(ShopDbContext db) => _db = db;

    [HttpGet("{clothingId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForClothing(int clothingId)
    {
        var reviews = await _db.Reviews
            .Where(r => r.ClothingId == clothingId)
            .Include(r => r.User)
            .Select(r => new { r.Id, r.Rating, r.Text, r.CreatedAt, UserName = r.User.Login })
            .ToListAsync();

        var avg = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        return Ok(new { AverageRating = avg, Reviews = reviews });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateReviewRequest request)
    {
        var userId = int.Parse(User.FindFirst("sub")!.Value);

        if (await _db.Reviews.AnyAsync(r => r.UserId == userId && r.ClothingId == request.ClothingId))
            return BadRequest("Вы (ты) уже оставил (и) отзыв на этот товар");

        _db.Reviews.Add(new Review
        {
            UserId = userId,
            ClothingId = request.ClothingId,
            Rating = request.Rating,
            Text = request.Text
        });
        await _db.SaveChangesAsync();

        // Обновляем среднюю оценку у товара
        var avg = await _db.Reviews.Where(r => r.ClothingId == request.ClothingId).AverageAsync(r => r.Rating);
        var clothing = await _db.Clothings.FindAsync(request.ClothingId);
        if (clothing != null)
        {
            clothing.AverageRating = avg;
            await _db.SaveChangesAsync();
        }

        return Ok(new { message = "Отзыв добавлен" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review == null) return NotFound();

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Отзыв удален" });
    }
}