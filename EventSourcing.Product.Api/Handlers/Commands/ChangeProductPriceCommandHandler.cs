using EventSourcing.Product.Api.CQRS;
using MediatR;

namespace EventSourcing.Product.Api;

public class ChangeProductPriceCommandHandler(ProductStream productStream) : IRequestHandler<ChangeProductPriceCommand>
{
    private readonly ProductStream _productStream = productStream;

    public async Task<Unit> Handle(ChangeProductPriceCommand request, CancellationToken cancellationToken)
    {
        _productStream.PriceChanged(request.ChangeProductPrice);

        await _productStream.SaveAsync();

        return Unit.Value;
    }
}