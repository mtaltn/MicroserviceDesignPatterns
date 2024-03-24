using A.Api.Models;

namespace A.Api.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _client;
    private readonly ILogger<ProductService> _logger;

    public ProductService(HttpClient client, ILogger<ProductService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<ProductDto> GetProduct(int id)
    {
        var product = await _client.GetFromJsonAsync<ProductDto>($"{id}");

        _logger.LogInformation($"Products:{product.Id}-{product.Name}");
        return product;
    }
}
