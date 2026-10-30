namespace ShopApi.DTO;

public record CreateProductRequest(string Name, decimal Price);
public record ProductResponse(int Id, string Name, decimal Price, bool IsAvailable);
public record LoginRequest(string Login, string Password);
public record RegisterRequest(string Login, string Password, string Phone, string Address);
public record CreateOrderRequest(List<OrderItemDto> Items);
public record OrderItemDto(int SizeId, int Quantity);
public record CreateCategoryRequest(string Name);
public record AddToCartRequest(int ClothingId, int Quantity);
public record UpdateCartRequest(int Quantity);
public record CreateReviewRequest(int ClothingId, int Rating, string Text);
public record CreatePickupPointRequest(string Name, string Address);
public record CreateOrderRequest2(List<CartItemDto> Items, string DeliveryType, string? DeliveryAddress, int? PickupPointId);
public record CartItemDto(int ClothingId, int Quantity);