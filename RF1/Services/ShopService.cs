using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Dtos;
using RF1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RF1.Services
{
    public class ShopsService : IShopsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ShopsService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShopDto>> GetShops()
        {
            var shops = await _context.Shops.ToListAsync();
            return _mapper.Map<List<ShopDto>>(shops);
        }

        public async Task<ShopDto> GetShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            return _mapper.Map<ShopDto>(shop);
        }

        public async Task<IEnumerable<ShopDto>> GetShopsByUserId(string userId)
        {
            var shops = await _context.Shops
                .Where(s => s.UserId == userId)
                .ToListAsync();

            return _mapper.Map<List<ShopDto>>(shops);
        }

        public async Task<ShopFullInfoDto> GetShopOrdersWithFarms(int shopId)
        {
            var shopOrdersWithFarms = await _context.Orders
                .Where(o => o.ShopId == shopId)
                .Select(o => new OrderFarmDto
                {
                    Id = o.Id,
                    Quantity = o.Quantity,
                    ShopPrice = o.ShopPrice,
                    SoldOut = o.SoldOut,
                    Status = o.Status,
                    ShopId = o.ShopId,
                    ProductId = o.ProductId,
                    Product = new ShortProductDto
                    {
                        Id = o.Product.Id,
                        Name = o.Product.Name,
                        Description = o.Product.Description,
                        Type = o.Product.Type,
                        PricePerUnit = o.Product.PricePerUnit,
                        UnitOfMeasurement = o.Product.UnitOfMeasurement,
                        Image = o.Product.Image,
                        Rating = _context.Ratings
                            .Where(r => r.ProductId == o.Product.Id)
                            .Average(r => (double?)r.RatingValue) ?? 0,
                        DateUpdated = o.Product.DateUpdated

                    },
                    ShortFarm = new ShortFarmDto
                    {
                        Id = o.Product.FarmId,
                        Name = o.Product.Farm.Name,
                        PhotoId = o.Product.Farm.PhotoId
                    }
                })
                .ToListAsync();

            var shopDetails = await _context.Shops
                .Where(s => s.Id == shopId)
                .Select(s => new ShopFullInfoDto
                {
                    Id = s.Id,
                    Image = s.Image,
                    UserId = s.UserId,
                    Name = s.Name,
                    Description = s.Description,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Rating = s.Rating,
                    Orders = shopOrdersWithFarms
                })
                .FirstOrDefaultAsync();

            return shopDetails;
        }

        public async Task<ShopDto> CreateShop(ShopDto shopDto)
        {
            var shop = _mapper.Map<Shop>(shopDto);

            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();

            shopDto.Id = shop.Id;

            return shopDto;
        }

        public async Task<bool> UpdateShop(int id, ShopDto shopDto)
        {
            if (id != shopDto.Id)
            {
                return false;
            }

            var shopInDb = await _context.Shops.FirstOrDefaultAsync(s => s.Id == id);
            if (shopInDb == null)
            {
                return false;
            }

            _mapper.Map(shopDto, shopInDb);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        public async Task<bool> DeleteShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
            {
                return false;
            }

            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();

            return true;
        }

        private bool ShopExists(int id)
        {
            return _context.Shops.Any(e => e.Id == id);
        }
    }
}
