namespace Order.Api.Models;

public record PaymentDto(string CardName, string CardNumber, string Expiration, string CVV);
