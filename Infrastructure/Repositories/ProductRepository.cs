using ProductApi.Domain.Entities;

namespace ProductApi.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
}

public class ProductRepository : IProductRepository
{
    private static readonly List<Product> Products =
    [
        new() { Id = 1, Name = "Keyboard", Price = 250 },
        new() { Id = 2, Name = "Mouse", Price = 150 },
        new() { Id = 3, Name = "Monitor", Price = 1200 },
    ];

    public Task<Product?> GetByIdAsync(int id)
    {
        return Task.FromResult(Products.FirstOrDefault(p => p.Id == id));
    }
}

