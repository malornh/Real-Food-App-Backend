using Microsoft.AspNetCore.Mvc;
using RF1.Dtos;
using RF1.Models;
using RF1.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RF1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productsService.GetProducts();
            return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productsService.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProduct = await _productsService.CreateProduct(productDto);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest("Mismatched IDs");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure that the FarmId provided is valid
            var farmExists = await _productsService.ValidateFarmId(productDto.FarmId);
            if (!farmExists)
            {
                return BadRequest("Farm ID is not valid");
            }

            var result = await _productsService.UpdateProduct(id, productDto);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productsService.DeleteProduct(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
