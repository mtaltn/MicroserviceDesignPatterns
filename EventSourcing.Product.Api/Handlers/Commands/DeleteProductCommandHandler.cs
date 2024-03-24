using EventSourcing.Product.Api.CQRS;
using MediatR;

namespace EventSourcing.Product.Api;

public class DeleteProductCommandHandler(ProductStream productStream) : IRequestHandler<DeleteProductCommand>
{
    private readonly ProductStream _productStream = productStream;

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _productStream.Deleted(request.Id);

        await _productStream.SaveAsync();

        return Unit.Value;
    }
}