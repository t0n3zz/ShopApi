using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.DTO;
using ShopApi.Models;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/pickup-points")]
public class PickupPointsController : ControllerBase
{
    private readonly ShopDbContext _db;
    public PickupPointsController(ShopDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await _db.PickupPoints.ToListAsync());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreatePickupPointRequest request)
    {
        var pp = new PickupPoint { Name = request.Name, Address = request.Address };
        _db.PickupPoints.Add(pp);
        await _db.SaveChangesAsync();
        return Ok(pp);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreatePickupPointRequest request)
    {
        var pp = await _db.PickupPoints.FindAsync(id);
        if (pp == null) return NotFound();

        pp.Name = request.Name;
        pp.Address = request.Address;
        await _db.SaveChangesAsync();
        return Ok(pp);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var pp = await _db.PickupPoints.FindAsync(id);
        if (pp == null) return NotFound();

        _db.PickupPoints.Remove(pp);
        await _db.SaveChangesAsync();
        return Ok(new { message = "ПВЗ удален" });
    }
}