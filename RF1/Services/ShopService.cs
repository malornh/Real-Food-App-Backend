using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Dtos;
using RF1.Models;
using RF1.Services.PhotoClients;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RF1.Services
{
    public class ShopsService : IShopsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public ShopsService(DataContext context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
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
                        PhotoId = o.Product.PhotoId,
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
                    PhotoId = s.PhotoId,
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

            var photoId = await _photoService.StorePhotoAsync(shopDto.PhotoFile, shopDto.UserId);
            shop.PhotoId = photoId;

            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();

            shopDto.Id = shop.Id;

            return shopDto;
        }

        public async Task<ShopDto> UpdateShop(int id, [FromForm]  ShopDto shopDto)
        {
            var shopInDb = await _context.Shops.FirstOrDefaultAsync(s => s.Id == id);
            if (shopInDb == null) throw new ArgumentNullException("Shop not found");

            _mapper.Map(shopDto, shopInDb);
            shopInDb.Id = id;

            if (shopDto.PhotoFile != null)
            {
                if (!string.IsNullOrEmpty(shopInDb.PhotoId))
                {
                    shopInDb.PhotoId = await _photoService.UpdatePhotoAsync(shopDto.PhotoFile, shopInDb.PhotoId, shopInDb.UserId);
                }
                else
                {
                    shopInDb.PhotoId = await _photoService.StorePhotoAsync(shopDto.PhotoFile, shopInDb.UserId);
                }
            }

            await _context.SaveChangesAsync();

            _mapper.Map(shopInDb, shopDto);

            return shopDto;
        }

        public async Task DeleteShop(int id)
        {
            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Id == id);
            if (shop == null) throw new ArgumentNullException();

            if (shop.PhotoId != null)
            {
                await _photoService.DeletePhotoAsync(shop.PhotoId);
            }

            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();
        }
    }
}
