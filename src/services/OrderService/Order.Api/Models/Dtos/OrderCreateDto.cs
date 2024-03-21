namespace Order.Api.Models;

public record OrderCreateDto(List<OrderItemDto> OrderItems, PaymentDto Payment, AddressDto Address ,string BuyerId);
