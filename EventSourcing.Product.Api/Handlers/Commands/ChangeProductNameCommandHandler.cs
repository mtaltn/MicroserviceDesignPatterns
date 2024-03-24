using EventSourcing.Product.Api.CQRS;
using MediatR;

namespace EventSourcing.Product.Api;

public class ChangeProductNameCommandHandler(ProductStream productStream) : IRequestHandler<ChangeProducNameCommand>
{
    private readonly ProductStream _productStream = productStream;

    public async Task<Unit> Handle(ChangeProducNameCommand request, CancellationToken cancellationToken)
    {
        _productStream.NameChanged(request.ChangeProductName);

        await _productStream.SaveAsync();

        return Unit.Value;
    }
}