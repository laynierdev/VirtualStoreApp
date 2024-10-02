using VirtualStoreApp.Models;

namespace VirtualStoreApp.Services;

// IProductService.cs
public interface IProductService
{
    Task<ApiResponse<Product>> CreateProductAsync(Product product);
    Task<IEnumerable<Product>> GetProductsAsync();

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

    /**
     * Create one product by calling service
     * 
     */
    public async Task<ApiResponse<Product>> CreateProductAsync(Product product){
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/api/product", product);

            if (response.IsSuccessStatusCode)
            {
                var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
                return new ApiResponse<Product>
                {
                    Success = true,
                    Data = createdProduct,
                    Message = "Producto creado con éxito."
                };
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return new ApiResponse<Product>
                {
                    Success = false,
                    Message = $"Error al crear el producto: {errorMessage}"
                };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<Product>
            {
                Success = false,
                Message = $"Error inesperado: {ex.Message}"
            };
        }
    }

    /**
     * Get all products by calling service
     * TODO exception validation
     */
    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/api/product");
    
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<Product>>() ?? Array.Empty<Product>();
        }
    
        return null;
    }

    
    
    
}
