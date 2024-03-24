namespace EventSourcing.Product.Api.Models;

public record ProductDto(Guid Id, int UserId, string Name, int Stock, decimal Price)
{
    public ProductDto(Guid id, int userId) : this(id, userId, "", 0, 0) { }

    public ProductDto() : this(Guid.Empty, 0, "", 0, 0) { }
}
