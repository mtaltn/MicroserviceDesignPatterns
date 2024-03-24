using A.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace A.Api.Controller;

[Route("api/[controller]")]
[ApiController]
public class ProdocutsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProdocutsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _productService.GetProduct(id));
    }
}
