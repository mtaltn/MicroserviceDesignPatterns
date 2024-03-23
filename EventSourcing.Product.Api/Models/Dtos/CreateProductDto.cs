namespace EventSourcing.Product.Api.Models;

public record CreateProductDto(int UserId, string Name, int Stock, decimal Price);
