using Microsoft.AspNetCore.Mvc;
using VirtualStoreApp.Models;

namespace VirtualStoreApp.controllers;

[ApiController]
[Route("server/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public ProductController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromForm] Product product)
    {
        if (product == null)
            return BadRequest("Invalid product data.");

        string imageUrl = null;

        if (product.Image != null && product.Image.Length > 0)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + product.Image.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await product.Image.CopyToAsync(fileStream);
            }

            imageUrl = $"/uploads/{uniqueFileName}";
        }
        product.ImageUrl = imageUrl;

        // Lógic to save to database by
        //calling a service to make a post to api/product

        return Ok(product);
    }
    
    [HttpGet("test")]
    public IActionResult TestEndpoint()
    {
        return Ok("Server is running!");
    }

}


