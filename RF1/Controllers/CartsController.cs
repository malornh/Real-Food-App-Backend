using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
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
    public class CartsController : ControllerBase
    {
        private readonly ICartsService _cartService;
        private readonly IProductsService _productService;
        private readonly IShopsService _shopService;

        public CartsController(ICartsService cartService, IProductsService productService, IShopsService shopService)
        {
            _cartService = cartService;
            _productService = productService;
            _shopService = shopService;
        }

        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDto>>> GetCarts()
        {
            var carts = await _cartService.GetCarts();

            return Ok(carts);
        }

        // GET: api/Carts/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<CartDto>> GetCart(int id)
        {
            var cart = await _cartService.GetCart(id);

            return Ok(cart);
        }

        // GET: api/Carts/UserCarts
        [Authorize]
        [HttpGet("UserCarts")]
        public async Task<ActionResult<IEnumerable<CartDto>>> GetCartsByUserId()
        {
            var carts = await _cartService.GetCartsByUserId();

            return Ok(carts);
        }

        // POST: api/Carts
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CartDto>> CreateCart([FromForm] int orderId)
        {
            var createdCart = await _cartService.CreateCart(orderId);

            return CreatedAtAction(nameof(GetCart), new { id = createdCart.Id }, createdCart);
        }

        // PUT: api/Carts/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromForm] int quantity)
        {
            var updatedCart = await _cartService.UpdateCart(id, quantity);

            return Ok(updatedCart);
        }

        // DELETE: api/Carts/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            await _cartService.DeleteCart(id);

            return NoContent();
        }
    }
}
