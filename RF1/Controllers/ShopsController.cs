using Microsoft.AspNetCore.Mvc;
using RF1.Dtos;
using RF1.Models;
using RF1.Services;
using System.Collections.Generic;

namespace RF1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : ControllerBase
    {
        private readonly IShopsService _shopsService;

        public ShopsController(IShopsService shopsService)
        {
            _shopsService = shopsService;
        }

        // GET: api/Shops
        [HttpGet]
        public ActionResult<IEnumerable<ShopDto>> GetShops()
        {
            var shops = _shopsService.GetShops();
            return Ok(shops);
        }

        // GET: api/Shops/5
        [HttpGet("{id}")]
        public ActionResult<ShopDto> GetShop(int id)
        {
            var shop = _shopsService.GetShop(id);
            if (shop == null)
            {
                return NotFound();
            }
            return Ok(shop);
        }

        // GET: api/Shops/ByUser/5
        [HttpGet("ByUser/{userId}")]
        public ActionResult<IEnumerable<ShopDto>> GetShopsByUserId()
        {
            var shops = _shopsService.GetShopsByUserId();

            return Ok(shops);
        }

        // GET: api/Shops/{shopId}/OrdersWithFarms
        [HttpGet("{shopId}/OrdersWithFarms")]
        public ActionResult<ShopFullInfoDto> GetShopOrdersWithFarms(int shopId)
        {
            var shopDetails = _shopsService.GetShopOrdersWithFarms(shopId);
            if (shopDetails == null)
            {
                return NotFound("No data found for the shop");
            }
            return Ok(shopDetails);
        }

        // POST: api/Shops
        [HttpPost]
        public ActionResult<ShopDto> CreateShop(ShopDto shopDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdShop = _shopsService.CreateShop(shopDto);
            return CreatedAtAction(nameof(GetShop), new { id = createdShop.Id }, createdShop);
        }

        // PUT: api/Shops/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ShopDto>> UpdateShop(int id, ShopDto shopDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            shopDto = await _shopsService.UpdateShop(id, shopDto);

            return Ok(shopDto);
        }

        // DELETE: api/Shops/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShop(int id)
        {
            await _shopsService.DeleteShop(id);

            return NoContent();
        }
    }
}
