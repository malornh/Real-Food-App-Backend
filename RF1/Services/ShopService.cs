using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Dtos;
using RF1.Models;
using RF1.Services.PhotoClients;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<ShopDto> GetShops()
        {
            var shops = _context.Shops.ToList();
            return _mapper.Map<List<ShopDto>>(shops);
        }

        public ShopDto GetShop(int id)
        {
            var shop = _context.Shops.Find(id);
            return _mapper.Map<ShopDto>(shop);
        }

        public IEnumerable<ShopDto> GetShopsByUserId(string userId)
        {
            var shops = _context.Shops
                .Where(s => s.UserId == userId)
                .ToList();

            return _mapper.Map<List<ShopDto>>(shops);
        }

        public ShopFullInfoDto GetShopOrdersWithFarms(int shopId)
        {
            var shopOrdersWithFarms = _context.Orders
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
                .ToList();

            var shopDetails = _context.Shops
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
                .FirstOrDefault();

            return shopDetails;
        }

        public ShopDto CreateShop(ShopDto shopDto)
        {
            var shop = _mapper.Map<Shop>(shopDto);

            var photoId = _photoService.StorePhotoAsync(shopDto.PhotoFile).GetAwaiter().GetResult();
            shop.PhotoId = photoId;

            _context.Shops.Add(shop);
            _context.SaveChanges();

            shopDto.Id = shop.Id;

            return shopDto;
        }

        public ShopDto UpdateShop(int id, ShopDto shopDto)
        {
            var shopInDb = _context.Shops.FirstOrDefault(s => s.Id == id);
            if (shopInDb == null) throw new ArgumentNullException("Shop not found");

            _mapper.Map(shopDto, shopInDb);
            shopInDb.Id = id;

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

        public void DeleteShop(int id)
        {
            var shop = _context.Shops.FirstOrDefault(s => s.Id == id);
            if (shop == null) throw new ArgumentNullException();

            if (shop.PhotoId != null)
            {
                _photoService.DeletePhotoAsync(shop.PhotoId).GetAwaiter().GetResult();
            }

            _context.Shops.Remove(shop);
            _context.SaveChanges();
        }
    }
}
