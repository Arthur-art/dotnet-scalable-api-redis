using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}

