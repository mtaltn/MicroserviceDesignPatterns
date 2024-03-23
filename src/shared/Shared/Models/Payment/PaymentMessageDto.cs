namespace Shared.Models;

public record PaymentMessageDto(string CardName, string CardNumber, string Expiration, string CVV, decimal TotalPrice)
{
    //bu neden böyle oldu öğrenilecek normalde buna gerek olmaması lazımdı
    public PaymentMessageDto() : this("", "", "", "", 0)
    {
    }
}
