using EventSourcing.Product.Api.CQRS;
using EventSourcing.Product.Api.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Product.Api;

public class GetProductAllListByUserIdHandler(AppDbContext context) : IRequestHandler<GetProductAllListByUserId, List<ProductDto>>
{
    private readonly AppDbContext _context = context;

    public async Task<List<ProductDto>> Handle(GetProductAllListByUserId request, CancellationToken cancellationToken)
    {
        var products = await _context.Products.Where(x => x.UserId == request.UserId).ToListAsync();

        return products.Select(x => new ProductDto { Id = x.Id, Name = x.Name, Price = x.Price, Stock = x.Stock, UserId = x.UserId }).ToList();
    }
}