﻿using AutoMapper;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace RF1.Services
{
    public class FarmsService : IFarmsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IFreeImageHostService _photoService;
        private readonly IUserAccessorService _userAccessorService;

        public FarmsService(DataContext context, IMapper mapper, IFreeImageHostService photoService, IUserAccessorService userAccessorService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
            _userAccessorService = userAccessorService;
        }

        public async Task<IEnumerable<FarmDto>> GetAllFarmsAsync()
        {
            var farms = await _context.Farms.ToListAsync();

            return _mapper.Map<List<FarmDto>>(farms);
        }

        public async Task<IEnumerable<FarmDto>> GetFarmsByIdsAsync(string farmIds)
        {
            var farmIdsArray = farmIds.Split(',').Select(int.Parse).ToArray();
            var farms = await _context.Farms
                .Where(f => farmIdsArray.Contains(f.Id))
                .ToListAsync();

            return _mapper.Map<List<FarmDto>>(farms);
        }

        public async Task<IEnumerable<FarmDto>> GetFarmsByUserIdAsync()
        {
            var userId = _userAccessorService.GetUserId();
            if (userId == null) throw new UnauthorizedAccessException("User not found.");

            var farms = await _context.Farms
                .Where(f => f.UserId == userId)
                .ToListAsync();

            if (farms == null || farms.Count == 0)
            {
                return new List<FarmDto>();
            }

            return _mapper.Map<List<FarmDto>>(farms);
        }

        public async Task<FarmDto> GetFarmByIdAsync(int id)
        {
            var farm = await _context.Farms.FirstOrDefaultAsync(s => s.Id == id);
            if (farm == null) throw new KeyNotFoundException($"Farm with {id} not found");

            return _mapper.Map<FarmDto>(farm);
        }

        public async Task<FarmFullInfoDto> GetFarmWithProductsAsync(int farmId)
        {
            var farmProducts = await _context.Products
                .Where(p => p.FarmId == farmId)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Type = p.Type,
                    PricePerUnit = p.PricePerUnit,
                    UnitOfMeasurement = p.UnitOfMeasurement,
                    PhotoUrl = p.PhotoUrl,
                    FarmId = p.FarmId,
                    Quantity = p.Quantity ?? 0,
                    DeliveryRadius = p.DeliveryRadius ?? 0,
                    MinUnitOrder = p.MinUnitOrder,
                    DateUpdated = p.DateUpdated,
                    Rating = _context.Ratings
                        .Where(r => r.ProductId == p.Id)
                        .Average(r => r.RatingValue)
                })
                .ToListAsync();

            var farmDetails = await _context.Farms
                .Where(f => f.Id == farmId)
                .Select(f => new FarmFullInfoDto
                {
                    Id = f.Id,
                    PhotoUrl = f.PhotoUrl,
                    UserId = f.UserId,
                    Name = f.Name,
                    Description = f.Description,
                    Latitude = f.Latitude,
                    Longitude = f.Longitude,
                    Rating = f.Rating,
                    Products = farmProducts
                })
                .FirstOrDefaultAsync();

            return farmDetails;
        }

        public async Task<FarmDto> CreateFarm(FarmDto farmDto)
        {
            var userId = _userAccessorService.GetUserId();
            farmDto.UserId = userId;

            var farm = _mapper.Map<Farm>(farmDto);

            farm.PhotoUrl = await _photoService.StorePhotoAsync(farmDto.PhotoFile);

            _context.Farms.Add(farm);
            await _context.SaveChangesAsync();

            farmDto.Id = farm.Id;
            farmDto.PhotoUrl = farm.PhotoUrl;
            return farmDto;
        }

        [HttpPut("{id}")]
        public async Task<FarmDto> UpdateFarm(int id, [FromForm] FarmDto farmDto)
        {
            var farmInDb = await _context.Farms.FirstOrDefaultAsync(f => f.Id == id);
            if (farmInDb == null) throw new KeyNotFoundException($"Farm with {id} not found");

            var userId = _userAccessorService.GetUserId();
            if (userId != farmInDb.UserId) throw new ArgumentException("User cannot edit other user's farm");

            farmInDb.Name = farmDto.Name;
            farmInDb.Description = farmDto.Description;
            farmInDb.Latitude = farmDto.Latitude;
            farmInDb.Longitude = farmDto.Longitude;
            farmInDb.DefaultDeliveryRadius = farmDto.DefaultDeliveryRadius;

            if (farmDto.PhotoFile != null)
            {
                if (!string.IsNullOrEmpty(farmInDb.PhotoUrl))
                {
                    farmInDb.PhotoUrl = await _photoService.UpdatePhotoAsync(farmDto.PhotoFile);
                }
                else
                {
                    farmInDb.PhotoUrl = await _photoService.StorePhotoAsync(farmDto.PhotoFile);
                }
            }

            await _context.SaveChangesAsync();

            _mapper.Map(farmInDb, farmDto);

            return farmDto;
        }

        public async Task DeleteFarm(int id)
        {
            var farmInDb = await _context.Farms.FirstOrDefaultAsync(f => f.Id == id);
            if (farmInDb == null) throw new ArgumentNullException("Farm not found.");

            var userId = _userAccessorService.GetUserId();
            if (userId != farmInDb.UserId) throw new UnauthorizedAccessException("User cannot delete another user's farm.");

            if (farmInDb.PhotoUrl != null)
            {
                await _photoService.DeletePhotoAsync(farmInDb.PhotoUrl);
            }

            _context.Farms.Remove(farmInDb);
            await _context.SaveChangesAsync();
        }
    }
}
