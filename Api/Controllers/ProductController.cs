using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.Services;

namespace ProductApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _service;

    public ProductController(ProductService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetProductAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }
}

