using Frontend.Web.Models;
using Frontend.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Web.Pages.Products;

public class IndexModel : PageModel
{
    private readonly ProductApiClient _products;

    public IndexModel(ProductApiClient products)
    {
        _products = products;
    }

    public List<Product> Items { get; private set; } = new();
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync()
    {
        try
        {
            Items = await _products.GetAllAsync();
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            ErrorMessage = "Products could not be loaded. Please try again later.";
        }
    }
}
