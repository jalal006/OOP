using Backend.Api.Data;
using Backend.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id).AsTask();
    }

    public async Task<Product> AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
        var existingProduct = await _context.Products.FindAsync(id);

        if (existingProduct is null)
        {
            return null;
        }

        existingProduct.ProductName = product.ProductName;
        existingProduct.Category = product.Category;
        existingProduct.UnitPrice = product.UnitPrice;
        existingProduct.UnitsInStock = product.UnitsInStock;
        existingProduct.Discontinued = product.Discontinued;

        await _context.SaveChangesAsync();
        return existingProduct;
    }

    public async Task<IEnumerable<Product>> SearchAsync(string term)
    {
        return await _context.Products
            .Where(p => p.ProductName.Contains(term))
            .ToListAsync();
    }
}
