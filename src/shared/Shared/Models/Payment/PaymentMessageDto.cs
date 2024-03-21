namespace Shared.Models;

public record PaymentMessageDto(string CardName, string CardNumber, string Expiration, string CVV, decimal TotalPrice);
