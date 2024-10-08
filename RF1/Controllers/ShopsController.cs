﻿using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<IEnumerable<ShopDto>>> GetShops()
        {
            var shops = await _shopsService.GetShops();

            return Ok(shops);
        }

        // GET: api/Shops/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShopDto>> GetShop(int id)
        {
            var shop = await _shopsService.GetShop(id);

            return Ok(shop);
        }

        // GET: api/Shops/ByUser
        [HttpGet("UserShops")]
        public async Task<ActionResult<IEnumerable<ShopDto>>> GetShopsByUserId()
        {
            var shops = await _shopsService.GetShopsByUserId();

            return Ok(shops);
        }

        // GET: api/Shops/{shopId}/OrdersWithFarms
        [HttpGet("{shopId}/OrdersWithFarms")]
        public async Task<ActionResult<ShopFullInfoDto>> GetShopOrdersWithFarms(int shopId)
        {
            var shopDetails = await _shopsService.GetShopOrdersWithFarms(shopId);

            return Ok(shopDetails);
        }

        // POST: api/Shops
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ShopDto>> CreateShop(ShopDto shopDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdShop = await _shopsService.CreateShop(shopDto);

            return CreatedAtAction(nameof(GetShop), new { id = createdShop.Id }, createdShop);
        }

        // PUT: api/Shops/5
        [Authorize]
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
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShop(int id)
        {
            await _shopsService.DeleteShop(id);

            return NoContent();
        }
    }
}
