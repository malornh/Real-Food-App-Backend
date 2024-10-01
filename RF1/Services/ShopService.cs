using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Dtos;
using RF1.Models;
using RF1.Services.PhotoClients;
using RF1.Services.UserAccessorService;
using System.Collections.Generic;
using System.Linq;

namespace RF1.Services
{
    public class ShopsService : IShopsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUserAccessorService _userAccessorService;

        public ShopsService(DataContext context, IMapper mapper, IPhotoService photoService, IUserAccessorService userAccessorService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
            _userAccessorService = userAccessorService;
        }

        public async Task<IEnumerable<ShopDto>> GetShops()
        {
            var shops = await _context.Shops.ToListAsync();

            return _mapper.Map<List<ShopDto>>(shops);
        }

        public async Task<ShopDto> GetShop(int id)
        {
            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Id == id);
            if (shop == null) throw new ArgumentNullException("Shop not found."); 

            return _mapper.Map<ShopDto>(shop);
        }

        public async Task<IEnumerable<ShopDto>> GetShopsByUserId()
        {
            var userId = _userAccessorService.GetUserId();

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
            var userId = _userAccessorService.GetUserId();
            shop.UserId = userId;

            var photoId = _photoService.StorePhotoAsync(shopDto.PhotoFile).GetAwaiter().GetResult();
            shop.PhotoId = photoId;

            _context.Shops.Add(shop);
            _context.SaveChanges();

            shopDto.Id = shop.Id;

            return shopDto;
        }


        public async Task<ShopDto> UpdateShop(int id, ShopDto shopDto)
        {
            var shopInDb = await _context.Shops.FirstOrDefaultAsync(s => s.Id == shopDto.Id);
            if (shopInDb == null) throw new ArgumentNullException("Shop not found");

            var userId = _userAccessorService.GetUserId();
            if (shopDto.UserId != userId) throw new UnauthorizedAccessException("User cannot edit another user's shop.");

            shopInDb.Name = shopDto.Name;
            shopInDb.Description = shopDto.Description;
            shopInDb.Latitude = shopDto.Latitude;
            shopInDb.Longitude = shopDto.Longitude;

            if (shopDto.PhotoFile != null)
            {
                if (!string.IsNullOrEmpty(shopInDb.PhotoId))
                {
                    shopInDb.PhotoId = _photoService.UpdatePhotoAsync(shopDto.PhotoFile, shopInDb.PhotoId).GetAwaiter().GetResult();
                }
                else
                {
                    shopInDb.PhotoId = _photoService.StorePhotoAsync(shopDto.PhotoFile).GetAwaiter().GetResult();
                }
            }

            _context.SaveChanges();
            _mapper.Map(shopInDb, shopDto);

            return shopDto;
        }

        public async Task DeleteShop(int id)
        {
            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Id == id);
            if (shop == null) throw new ArgumentNullException("Shop not found.");

            var userId = _userAccessorService.GetUserId();
            if (shop.UserId != userId) throw new UnauthorizedAccessException("User cannot edit another user's shop.");

            if (shop.PhotoId != null)
            {
                _photoService.DeletePhotoAsync(shop.PhotoId).GetAwaiter().GetResult();
            }

            _context.Shops.Remove(shop);
             await _context.SaveChangesAsync();
        }
    }
}
