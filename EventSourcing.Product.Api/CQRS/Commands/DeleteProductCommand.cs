using MediatR;

namespace EventSourcing.Product.Api.CQRS;

public class DeleteProductCommand : IRequest
{
    public Guid Id { get; set; }
}
