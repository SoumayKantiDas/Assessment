using System;
using System.Collections.Generic;
using Assessment.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}


public static class ProductEndpoints
{
	public static void MapProductEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Product").WithTags(nameof(Product));

        group.MapGet("/", async (AssessmentContext db) =>
        {
            return await db.Products.ToListAsync();
        })
        .WithName("GetAllProducts")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Product>, NotFound>> (int productid, AssessmentContext db) =>
        {
            return await db.Products.AsNoTracking()
                .FirstOrDefaultAsync(model => model.ProductId == productid)
                is Product model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetProductById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int productid, Product product, AssessmentContext db) =>
        {
            var affected = await db.Products
                .Where(model => model.ProductId == productid)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.ProductId, product.ProductId)
                  .SetProperty(m => m.Name, product.Name)
                  .SetProperty(m => m.Description, product.Description)
                  .SetProperty(m => m.CreatedAt, product.CreatedAt)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateProduct")
        .WithOpenApi();

        group.MapPost("/", async (Product product, AssessmentContext db) =>
        {
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Product/{product.ProductId}",product);
        })
        .WithName("CreateProduct")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int productid, AssessmentContext db) =>
        {
            var affected = await db.Products
                .Where(model => model.ProductId == productid)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteProduct")
        .WithOpenApi();
    }
}