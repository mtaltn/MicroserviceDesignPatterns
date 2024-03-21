namespace Order.Api.Models;

public record OrderItemDto(int ProductId, int Count, decimal Price);
