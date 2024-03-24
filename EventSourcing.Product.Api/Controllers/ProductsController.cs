using EventSourcing.Product.Api.CQRS;
using EventSourcing.Product.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.Product.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    // controllarda primary constructor kullanmak pek hoşuma gitmiyor ama isterseniz uygulayabilirsiniz
    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetAllListByUserId(int userId) => Ok(await _mediator.Send(new GetProductAllListByUserId() { UserId = userId }));

    ///üstteki kod ile tamamen olmasa da işlevsel olarak aynıdır alternatif olsun diye eklenmiştir
    ///bir RESTful API tasarlarken, kaynakları temsil eden rotaları kullanarak parametreleri belirtmek standart bir uygulamadır
    //[HttpGet("{userId}")]
    //public async Task<IActionResult> GetAllListByUserId([FromRoute] int userId) => Ok(await _mediator.Send(new GetProductAllListByUserId() { UserId = userId }));

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto createProductDto) =>
    await _mediator.Send(new CreateProductCommand { CreateProduct = createProductDto }) != null ? NoContent() : NotFound();

    [HttpPut(nameof(ChangeName))]
    public async Task<IActionResult> ChangeName(ChangeProductNameDto changeProductNameDto) =>
    await _mediator.Send(new ChangeProducNameCommand { ChangeProductName = changeProductNameDto }) != null ? NoContent() : NotFound();

    [HttpPut(nameof(ChangePrice))]
    public async Task<IActionResult> ChangePrice(ChangeProductPriceDto changeProductPriceDto) =>
        await _mediator.Send(new ChangeProductPriceCommand { ChangeProductPrice = changeProductPriceDto }) != null ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) =>
        (await _mediator.Send(new DeleteProductCommand { Id = id })) != null ? NoContent() : NotFound();

    #region Check Here for Alternative Code
    ///---------------
    ///üstte kısa yazımları bulunan kodların okunabilirliği az olarak düşünüyorsanız bu şekilde kullanabilirisiniz.
    //[HttpPost]
    //public async Task<IActionResult> Create(CreateProductDto createProductDto)
    //{
    //    await _mediator.Send(new CreateProductCommand() { CreateProduct = createProductDto });
    //    return NoContent();
    //}

    //[HttpPut(nameof(ChangeName))]
    //public async Task<IActionResult> ChangeName(ChangeProductNameDto changeProductNameDto)
    //{
    //    await _mediator.Send(new ChangeProducNameCommand { ChangeProductName = changeProductNameDto });
    //    return NoContent();
    //}

    //[HttpPut(nameof(ChangePrice))]
    //public async Task<IActionResult> ChangePrice(ChangeProductPriceDto changeProductPriceDto)
    //{
    //    await _mediator.Send(new ChangeProductPriceCommand { ChangeProductPrice = changeProductPriceDto });
    //    return NoContent();
    //}

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> Delete(Guid id)
    //{
    //    await _mediator.Send(new DeleteProductCommand { Id = id });
    //    return NoContent();
    //}
    #endregion
}

