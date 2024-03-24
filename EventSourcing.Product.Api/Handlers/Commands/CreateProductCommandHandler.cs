using EventSourcing.Product.Api.CQRS;
using MediatR;

namespace EventSourcing.Product.Api;

public class CreateProductCommandHandler(ProductStream productStream) : IRequestHandler<CreateProductCommand>
{
    private readonly ProductStream _productStream = productStream;

    public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _productStream.Created(request.CreateProduct);

        await _productStream.SaveAsync();

        return Unit.Value;
    }
}