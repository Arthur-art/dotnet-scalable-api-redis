using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly IDistributedCache _cache;

    public ProductService(IProductRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        /* Primeiro tenta ler do cache (GetStringAsync).

        Se não encontrar, busca no repositório.

        Depois armazena no Redis por 5 minutos (com expiração absoluta). */

        var cacheKey = GetCacheKey(id);
        var cachedProduct = await _cache.GetStringAsync(cacheKey);

        if (cachedProduct is not null)
        {
            Console.WriteLine($"[CACHE HIT] Product {id} loaded from Redis.");
            var productFromCache = JsonSerializer.Deserialize<Product>(cachedProduct);
            if (productFromCache is not null)
            {
                return productFromCache;
            }
        }

        Console.WriteLine($"[CACHE MISS] Fetching Product {id} from repository...");
        var product = await _repository.GetByIdAsync(id);

        if (product is not null)
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
        }

        return product;
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        var created = await _repository.AddAsync(product);
        await _cache.RemoveAsync(GetCacheKey(created.Id));
        return created;
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        var updated = await _repository.UpdateAsync(product);
        if (updated)
        {
            await _cache.RemoveAsync(GetCacheKey(product.Id));
        }

        return updated;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (deleted)
        {
            await _cache.RemoveAsync(GetCacheKey(id));
        }

        return deleted;
    }

    private static string GetCacheKey(int id) => $"product:{id}";
}

