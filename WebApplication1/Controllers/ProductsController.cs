using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.DTO;
using ShopApi.Models;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/clothing")]
public class ProductsController : ControllerBase
{
    private readonly ShopDbContext _db;
    public ProductsController(ShopDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
    {
        var query = _db.Clothings.Include(c => c.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search));
        if (categoryId.HasValue)
            query = query.Where(c => c.CategoryId == categoryId.Value);
        if (minPrice.HasValue)
            query = query.Where(c => c.Price >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(c => c.Price <= maxPrice.Value);

        return Ok(await query.ToListAsync());
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _db.Clothings
            .Include(c => c.Category)
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Clothing product)
    {
        _db.Clothings.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] Clothing updated)
    {
        var product = await _db.Clothings.FindAsync(id);
        if (product == null) return NotFound();

        product.Name = updated.Name;
        product.Price = updated.Price;
        product.Description = updated.Description;
        product.CategoryId = updated.CategoryId;
        product.ImagePath = updated.ImagePath;
        product.PackQuantity = updated.PackQuantity;
        product.IsAvailable = updated.IsAvailable;

        await _db.SaveChangesAsync();
        return Ok(product);
    }

    [HttpPatch("{id}/stock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStock(int id, [FromQuery] int quantity)
    {
        var product = await _db.Clothings.FindAsync(id);
        if (product == null) return NotFound();

        product.PackQuantity = quantity;
        await _db.SaveChangesAsync();
        return Ok(product);
    }

    [HttpPatch("{id}/availability")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetAvailability(int id, [FromQuery] bool isAvailable)
    {
        var product = await _db.Clothings.FindAsync(id);
        if (product == null) return NotFound();

        product.IsAvailable = isAvailable;
        await _db.SaveChangesAsync();
        return Ok(product);
    }
}