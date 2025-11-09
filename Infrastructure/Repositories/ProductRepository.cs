using ProductApi.Domain.Entities;

namespace ProductApi.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
}

public class ProductRepository : IProductRepository
{
    private static readonly object SyncRoot = new();

    private static readonly List<Product> Products =
    [
        new() { Id = 1, Name = "Keyboard", Price = 250 },
        new() { Id = 2, Name = "Mouse", Price = 150 },
        new() { Id = 3, Name = "Monitor", Price = 1200 },
    ];

    public Task<IReadOnlyList<Product>> GetAllAsync()
    {
        lock (SyncRoot)
        {
            return Task.FromResult<IReadOnlyList<Product>>(Products.Select(Clone).ToList());
        }
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        lock (SyncRoot)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product is null ? null : Clone(product));
        }
    }

    public Task<Product> AddAsync(Product product)
    {
        lock (SyncRoot)
        {
            var nextId = Products.Count == 0 ? 1 : Products.Max(p => p.Id) + 1;
            var newProduct = new Product
            {
                Id = nextId,
                Name = product.Name,
                Price = product.Price
            };

            Products.Add(newProduct);

            return Task.FromResult(Clone(newProduct));
        }
    }

    public Task<bool> UpdateAsync(Product product)
    {
        lock (SyncRoot)
        {
            var existing = Products.FirstOrDefault(p => p.Id == product.Id);
            if (existing is null)
            {
                return Task.FromResult(false);
            }

            existing.Name = product.Name;
            existing.Price = product.Price;

            return Task.FromResult(true);
        }
    }

    public Task<bool> DeleteAsync(int id)
    {
        lock (SyncRoot)
        {
            var removed = Products.RemoveAll(p => p.Id == id) > 0;
            return Task.FromResult(removed);
        }
    }

    private static Product Clone(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price
    };
}

