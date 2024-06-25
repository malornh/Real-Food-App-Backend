using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Models;
using RF1.Dtos;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Threading.Tasks;

namespace RF1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public OrdersController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Orders
        [HttpGet]
        public IEnumerable<OrderDto> GetOrders()
        {
            var orders = _context.Orders.ToList();
            return _mapper.Map<List<OrderDto>>(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public ActionResult<OrderDto> GetOrder(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return _mapper.Map<OrderDto>(order);
        }

        [HttpGet("AllFarmOrders/{farmId}")]
        public async Task<ActionResult<List<AllFarmOrdersDto>>> GetAllFarmOrdersByFarmId(int farmId)
        {
            var pendingOrders = await _context.Orders
                                              .Where(o => o.Product.FarmId == farmId)
                                              .Include(o => o.Product)
                                              .ThenInclude(p => p.Farm)
                                              .Include(o => o.Shop)
                                              .ToListAsync();

            if (pendingOrders == null || pendingOrders.Count == 0)
            {
                return NotFound();
            }

            var orderDtos = pendingOrders.Select(o => new AllFarmOrdersDto
            {
                Id = o.Id,
                Status = o.Status,
                Quantity = o.Quantity,
                ShopPrice = o.ShopPrice,
                DateOrdered = o.DateOrdered,
                Product = new ProductDto
                {
                    Id = o.Product.Id,
                    Name = o.Product.Name,
                    Image = o.Product.Image,
                    PricePerUnit = o.Product.PricePerUnit,
                    Type = o.Product.Type,
                },
                Farm = new FarmDto
                {
                    Id = o.Product.Farm.Id,
                    Name = o.Product.Farm.Name,
                    Image = o.Product.Farm.Image,
                    Latitude = o.Product.Farm.Latitude,
                    Longitude = o.Product.Farm.Longitude,
                },
                Shop = new ShopDto
                {
                    Id = o.Shop.Id,
                    Name = o.Shop.Name,
                    Image = o.Shop.Image,
                    Latitude = o.Product.Farm.Latitude,
                    Longitude = o.Product.Farm.Longitude,
                }
            }).ToList();

            return Ok(orderDtos);
        }


        // POST: api/Orders
        [HttpPost]
        public IActionResult CreateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = _mapper.Map<Order>(orderDto);

            _context.Orders.Add(order);
            _context.SaveChanges();

            orderDto.Id = order.Id;

            return Created(new Uri(Request.GetDisplayUrl() + "/" + orderDto.Id), orderDto);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, OrderDto orderDto)
        {
            if (id != orderDto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderInDb = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (orderInDb == null)
            {
                return NotFound();
            }

            _mapper.Map(orderDto, orderInDb);

            _context.SaveChanges();

            return Ok(orderDto);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var orderInDb = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (orderInDb == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(orderInDb);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
