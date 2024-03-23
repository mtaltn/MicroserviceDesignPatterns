using EventSourcing.Product.Api.Models;
using MediatR;

namespace EventSourcing.Product.Api.CQRS;

public class CreateProductCommand : IRequest
{
    public CreateProductDto CreateProduct { get; set; }
}
