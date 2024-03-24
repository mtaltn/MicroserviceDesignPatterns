using Microsoft.AspNetCore.Mvc;

namespace B.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(new { Id = id, Name = "Kalem", Price = 100, Stock = 200, Category = "Kalemler" });
        }
    }
}
