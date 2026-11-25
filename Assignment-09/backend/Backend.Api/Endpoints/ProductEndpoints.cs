using Backend.Api.Models;
using Backend.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Backend.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async (IProductService productService) =>
        {
            var products = await productService.GetAllAsync();
            return Results.Ok(products);
        });

        app.MapGet("/api/products/{id:int}", async (int id, IProductService productService) =>
        {
            var product = await productService.GetByIdAsync(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        app.MapPost("/api/products", async (Product product, IProductService productService) =>
        {
            if (product is null)
            {
                return Results.BadRequest();
            }

            var createdProduct = await productService.AddAsync(product);
            return Results.Created($"/api/products/{createdProduct.ProductID}", createdProduct);
        });

        app.MapDelete("/api/products/{id:int}", async (int id, IProductService productService) =>
        {
            var wasDeleted = await productService.DeleteAsync(id);
            return wasDeleted ? Results.NoContent() : Results.NotFound();
        });

        app.MapPut("/api/products/{id:int}", async (int id, Product product, IProductService productService) =>
        {
            var updatedProduct = await productService.UpdateAsync(id, product);
            return updatedProduct is null ? Results.NotFound() : Results.Ok(updatedProduct);
        });

        app.MapGet("/api/products/search", async (string query, IProductService productService) =>
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Results.BadRequest("The search term is required.");
            }

            var products = await productService.SearchAsync(query);
            return Results.Ok(products);
        });

        return app;
    }
}
