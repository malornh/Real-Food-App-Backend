using Microsoft.AspNetCore.Authorization;
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
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        // GET: api/Orders
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _ordersService.GetOrders();
            return Ok(orders);
        }

        // GET: api/Orders/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _ordersService.GetOrder(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        // GET: api/Orders/AllFarmOrders/{farmId}
        [Authorize]
        [HttpGet("AllFarmOrders/{farmId}")]
        public async Task<ActionResult<List<AllFarmOrdersDto>>> GetAllFarmOrdersByFarmId(int farmId)
        {
            var farmOrders = await _ordersService.GetAllFarmOrdersByFarmId(farmId);
            if (farmOrders == null || farmOrders.Count == 0)
            {
                return NotFound();
            }
            return Ok(farmOrders);
        }

        // GET: api/Orders/AllShopOrders/{shopId}
        [Authorize]
        [HttpGet("AllShopOrders/{shopId}")]
        public async Task<ActionResult<List<AllShopOrdersDto>>> GetAllShopOrdersByShopId(int shopId)
        {
            var shopOrders = await _ordersService.GetAllShopOrdersByShopId(shopId);
            if (shopOrders == null || shopOrders.Count == 0)
            {
                return NotFound();
            }
            return Ok(shopOrders);
        }


        // POST: api/Orders
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdOrder = await _ordersService.CreateOrder(orderDto);
            return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
        }

        // PUT: api/Orders/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderDto orderDto)
        {
            var result = await _ordersService.UpdateOrder(id, orderDto);

            return Ok(orderDto);
        }

        // DELETE: api/Orders/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _ordersService.DeleteOrder(id);

            return NoContent();
        }
    }
}
