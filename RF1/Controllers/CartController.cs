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
    public class CartController : ControllerBase
    {
        private readonly ICartsService _cartService;
        private readonly IProductsService _productService;
        private readonly IShopsService _shopService;

        public CartController(ICartsService cartService, IProductsService productService, IShopsService shopService)
        {
            _cartService = cartService;
            _productService = productService;
            _shopService = shopService;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDto>>> GetCarts()
        {
            var carts = await _cartService.GetCarts();
            return Ok(carts);
        }

        // GET: api/Cart/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartDto>> GetCart(int id)
        {
            var cart = await _cartService.GetCart(id);
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }

        // GET: api/Cart/User/{userId}
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<CartDto>>> GetCartsByUserId()
        {
            var carts = await _cartService.GetCartsByUserId();
            return Ok(carts);
        }

        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<CartDto>> CreateCart(int productId, int shopId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCart = await _cartService.CreateCart(productId, shopId);
            if (createdCart == null)
            {
                return NotFound();
            }

            return CreatedAtAction(nameof(GetCart), new { id = createdCart.Id }, createdCart);
        }

        // PUT: api/Cart/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, CartDto cartDto)
        {
            var result = await _cartService.UpdateCart(id, cartDto);

            return Ok(cartDto);
        }

        // DELETE: api/Cart/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var result = await _cartService.DeleteCart(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
