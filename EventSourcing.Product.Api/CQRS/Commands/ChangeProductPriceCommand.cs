using EventSourcing.Product.Api.Models;
using MediatR;

namespace EventSourcing.Product.Api.CQRS;

public class ChangeProductPriceCommand : IRequest
{
    public ChangeProductPriceDto ChangeProductPrice { get; set; }
}
