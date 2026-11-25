using System.Net.Http.Json;
using Frontend.Web.Models;

namespace Frontend.Web.Services;

public class ProductApiClient
{
    private readonly HttpClient client;

    public ProductApiClient(HttpClient httpClient)
    {
        client = httpClient;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await client.GetFromJsonAsync<List<Product>>("api/products")
            ?? new List<Product>();
    }
}
