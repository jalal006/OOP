using Backend.Api.Models;

namespace Backend.Api.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<Product?> UpdateAsync(int id, Product product);
    Task<IEnumerable<Product>> SearchAsync(string term);
}
