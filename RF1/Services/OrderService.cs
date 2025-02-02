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
                    PhotoUrl = o.Product.PhotoUrl,
                    PricePerUnit = o.Product.PricePerUnit,
                    Type = o.Product.Type,
                },
                Farm = new FarmDto
                {
                    Id = o.Product.Farm.Id,
                    Name = o.Product.Farm.Name,
                    PhotoUrl = o.Product.Farm.PhotoUrl,
                    Latitude = o.Product.Farm.Latitude,
                    Longitude = o.Product.Farm.Longitude,
                },
                Shop = new ShopDto
                {
                    Id = o.Shop.Id,
                    Name = o.Shop.Name,
                    PhotoUrl = o.Shop.PhotoUrl,
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
                    PhotoUrl = o.Product.PhotoUrl,
                    PricePerUnit = o.Product.PricePerUnit,
                    Type = o.Product.Type,
                },
                Farm = new FarmDto
                {
                    Id = o.Product.Farm.Id,
                    Name = o.Product.Farm.Name,
                    PhotoUrl = o.Product.Farm.PhotoUrl,
                    Latitude = o.Product.Farm.Latitude,
                    Longitude = o.Product.Farm.Longitude,
                },
                Shop = new ShopDto
                {
                    Id = o.Shop.Id,
                    Name = o.Shop.Name,
                    PhotoUrl = o.Shop.PhotoUrl,
                    Latitude = o.Shop.Latitude,
                    Longitude = o.Shop.Longitude,
                }
            }).ToList();

            return orderDtos;
        }


        public async Task<OrderBulkDto> CreateOrder(OrderDto orderDto)
        {
            if (orderDto.ProductId == null) throw new ArgumentNullException("ProductId is required.");
            if (orderDto.ShopId == null) throw new ArgumentNullException("ShopId is required.");

            var orderInDb = _mapper.Map<Order>(orderDto);
            _context.Orders.Add(orderInDb);
            await _context.SaveChangesAsync();

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderInDb.ProductId);
            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Id == orderInDb.ShopId);
            var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == product.FarmId);

            var productDto = _mapper.Map<ProductDto>(product);
            var shopDto = _mapper.Map<ShopDto>(shop);
            var farmDto = _mapper.Map<FarmDto>(farm);

            var orderBulk = new OrderBulkDto
            {
                Id = orderInDb.Id,
                Status = orderInDb.Status,
                Quantity = orderInDb.Quantity,
                ShopPrice = (decimal?)orderInDb.ShopPrice,
                DateOrdered = orderInDb.DateOrdered.ToDateTime(new TimeOnly()),
                Product = productDto,
                Shop = shopDto,
                Farm = farmDto
            };

            return orderBulk;
        }

        [HttpPut("{id}")]
        public async Task<OrderDto> UpdateOrder(int id, OrderDto orderDto)
        {
            var orderInDb = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (orderInDb == null) throw new ArgumentNullException(nameof(orderDto));

            orderInDb.ShopPrice = orderDto.ShopPrice;
            orderInDb.Status = orderDto.Status;
            orderInDb.SoldOut = orderDto.SoldOut;

            await _context.SaveChangesAsync();

            _mapper.Map(orderInDb, orderDto);

            return orderDto;
        }
    }
}
