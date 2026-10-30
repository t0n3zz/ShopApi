using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.DTO;
using ShopApi.Models;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ShopDbContext _db;
    public OrdersController(ShopDbContext db) => _db = db;

    private int GetUserId() => int.Parse(User.FindFirst("sub")!.Value);

    // create order from cart
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest2 request)
    {
        var userId = GetUserId();
        var order = new Order
        {
            UserId = userId,
            Status = "Оформлен",
            CreatedAt = DateTime.UtcNow,
            DeliveryType = request.DeliveryType,
            DeliveryAddress = request.DeliveryAddress,
            PickupPointId = request.PickupPointId
        };

        decimal total = 0;

        foreach (var item in request.Items)
        {
            var clothing = await _db.Clothings.FindAsync(item.ClothingId);
            if (clothing == null || !clothing.IsAvailable)
                return BadRequest($"Товар {item.ClothingId} недоступен");
            if (clothing.PackQuantity == null || clothing.PackQuantity < item.Quantity)
                return BadRequest($"Недостаточно товара {clothing.Name}");

            clothing.PackQuantity -= item.Quantity;
            var price = clothing.Price;
            total += price * item.Quantity;

            order.Items.Add(new OrderItem
            {
                ClothingId = item.ClothingId,
                Quantity = item.Quantity,
                PriceAtPurchase = price
            });
        }

        if (total < 600 && request.DeliveryType == "Delivery")
            return BadRequest("Доставка доступна только для заказов от 600 рублей. Выберите ПВЗ.");
        if (request.DeliveryType == "Delivery" && string.IsNullOrWhiteSpace(request.DeliveryAddress))
            return BadRequest("Укажите адрес доставки");
        if (request.DeliveryType == "Pickup" && request.PickupPointId == null)
            return BadRequest("Выберите ПВЗ");

        order.TotalPrice = total;
        _db.Orders.Add(order);

        // clean busket
        var cartItems = await _db.CartItems.Where(c => c.UserId == userId).ToListAsync();
        _db.CartItems.RemoveRange(cartItems);

        await _db.SaveChangesAsync();
        return Created($"/api/orders/{order.Id}", new { order.Id, order.TotalPrice, order.Status });
    }

    // orders history for sorting
    [HttpGet]
    public async Task<IActionResult> GetHistory([FromQuery] string sort = "desc")
    {
        var userId = GetUserId();
        var query = _db.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Clothing)
            .AsQueryable();

        query = sort == "asc"
            ? query.OrderBy(o => o.CreatedAt)
            : query.OrderByDescending(o => o.CreatedAt);

        var orders = await query
            .Select(o => new
            {
                o.Id,
                o.CreatedAt,
                o.Status,
                o.TotalPrice,
                o.DeliveryType,
                o.DeliveryAddress,
                Items = o.Items.Select(i => new
                {
                    i.ClothingId,
                    Name = i.Clothing.Name,
                    i.Quantity,
                    PriceAtPurchase = i.PriceAtPurchase,
                    Total = i.PriceAtPurchase * i.Quantity
                })
            })
            .ToListAsync();

        return Ok(orders);
    }

    // only for adms
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string newStatus)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = newStatus;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Статус изменен", order });
    }
}