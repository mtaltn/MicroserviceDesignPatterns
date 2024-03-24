using A.Api.Models;

namespace A.Api.Services;

public interface IProductService
{
    Task<ProductDto> GetProduct(int id);
}
