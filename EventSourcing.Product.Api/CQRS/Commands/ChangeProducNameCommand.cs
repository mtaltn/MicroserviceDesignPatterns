using EventSourcing.Product.Api.Models;
using MediatR;

namespace EventSourcing.Product.Api.CQRS;

public class ChangeProducNameCommand : IRequest
{
    public ChangeProductNameDto ChangeProductName { get; set; }
}
