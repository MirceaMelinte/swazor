using Microsoft.AspNetCore.Mvc;

namespace Swazor.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesResponseType<Product>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var product = new Product(id, "Foo product", 29.99m);
        return Ok(product);
    }

    // This endpoint has no matching Products_Ping.cshtml template: test exercises WarnOnMissingTemplate
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("pong");

    [HttpPost]
    [ProducesResponseType<Product>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] CreateProductRequest request)
    {
        var product = new Product(42, request.Name, request.Price);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
}

public record Product(int Id, string Name, decimal Price);

public record CreateProductRequest(string Name, decimal Price);