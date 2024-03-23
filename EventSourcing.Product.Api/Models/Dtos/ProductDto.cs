namespace EventSourcing.Product.Api.Models;

public record ProductDto(Guid Id, int UserId, string Name, int Stock, decimal Price);
