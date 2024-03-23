using EventSourcing.Product.Api.Models;
using MediatR;

namespace EventSourcing.Product.Api.CQRS;

public class GetProductAllListByUserId : IRequest<List<ProductDto>>
{
    public int UserId { get; set; }
}