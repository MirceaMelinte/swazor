using Microsoft.AspNetCore.Mvc;

namespace Swazor.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController
    : ControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesResponseType<Product>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var product = new Product(id, "Foo product", 29.99m);

        return Ok(product);
    }
}

public record Product(int Id, string Name, decimal Price);
