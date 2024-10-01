using VirtualStoreApp.Models;

namespace VirtualStoreApp.Services;

// IProductService.cs
public interface IProductService
{
    Task<bool> CreateProductAsync(Product product);
}

// ProductService.cs
public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiUrl;

    public ProductService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiUrl = configuration["Api:ApiUrl"]  ?? throw new ArgumentNullException("ApiUrl not configured.");;
    }

    public async Task<bool> CreateProductAsync(Product product)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/api/product", product);
        return response.IsSuccessStatusCode;
    }
}
