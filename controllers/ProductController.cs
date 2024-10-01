using Microsoft.AspNetCore.Mvc;
using VirtualStoreApp.Models;
using VirtualStoreApp.Services;

namespace VirtualStoreApp.controllers;

[ApiController]
[Route("server/[controller]")]

public class ProductController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IProductService _productService;
 

    public ProductController(IWebHostEnvironment environment, IProductService productService)
    {
        _environment = environment;
        _productService = productService;
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

            // Verifica si el directorio existe, si no, lo crea
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + product.Image.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await product.Image.CopyToAsync(fileStream);
            }

            imageUrl = $"/uploads/{uniqueFileName}";
        }

        product.ImageUrl = imageUrl;
        
        //call service to store product
       var result = await _productService.CreateProductAsync(product);

       if (result)
       {
            return Ok(product); 
       }
       else
       {
           return BadRequest("Product could not be created.");
       }

      
    }
    
    [HttpGet("test")]
    public IActionResult TestEndpoint()
    {
        return Ok("Server is running!");
    }

}


