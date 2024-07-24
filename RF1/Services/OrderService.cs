using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Dtos;
using RF1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RF1.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public OrdersService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrder(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<List<AllFarmOrdersDto>> GetAllFarmOrdersByFarmId(int farmId)
        {
            var pendingOrders = await _context.Orders
                                              .Where(o => o.Product.FarmId == farmId)
                                              .Include(o => o.Product)
                                              .ThenInclude(p => p.Farm)
                                              .Include(o => o.Shop)
                                              .ToListAsync();

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

            return orderDtos;
        }

        public async Task<List<AllShopOrdersDto>> GetAllShopOrdersByShopId(int shopId)
        {
            var pendingOrders = await _context.Orders
                                              .Where(o => o.ShopId == shopId)
                                              .Include(o => o.Product)
                                              .ThenInclude(p => p.Farm)
                                              .Include(o => o.Shop)
                                              .ToListAsync();

            var orderDtos = pendingOrders.Select(o => new AllShopOrdersDto
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
                    Latitude = o.Shop.Latitude,
                    Longitude = o.Shop.Longitude,
                }
            }).ToList();

            return orderDtos;
        }


        public async Task<OrderDto> CreateOrder(OrderDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            orderDto.Id = order.Id;
            return orderDto;
        }

        public async Task<bool> UpdateOrder(int id, OrderDto orderDto)
        {
            if (id != orderDto.Id)
            {
                return false;
            }

            var orderInDb = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (orderInDb == null)
            {
                return false;
            }

            _mapper.Map(orderDto, orderInDb);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var orderInDb = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (orderInDb == null)
            {
                return false;
            }

            _context.Orders.Remove(orderInDb);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
