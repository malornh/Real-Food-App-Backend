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
    public class FarmsService : IFarmsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FarmsService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FarmDto>> GetFarms()
        {
            var farms = await _context.Farms.ToListAsync();
            return _mapper.Map<List<FarmDto>>(farms);
        }

        public async Task<IEnumerable<FarmDto>> GetFarmsByIds(string farmIds)
        {
            var farmIdsArray = farmIds.Split(',').Select(int.Parse).ToArray();
            var farms = await _context.Farms.Where(f => farmIdsArray.Contains(f.Id)).ToListAsync();
            return _mapper.Map<List<FarmDto>>(farms);
        }

        public async Task<IEnumerable<FarmDto>> GetFarmsByUserId(string userId)
        {
            var farms = await _context.Farms.Where(f => f.UserId == userId).ToListAsync();
            return _mapper.Map<List<FarmDto>>(farms);
        }

        public async Task<FarmDto> GetFarm(int id)
        {
            var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == id);
            return _mapper.Map<FarmDto>(farm);
        }

        public async Task<FarmFullInfoDto> GetFarmWithProducts(int farmId)
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
                    Image = p.Image,
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
                    Image = f.Image,
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
            var farm = _mapper.Map<Farm>(farmDto);
            _context.Farms.Add(farm);
            await _context.SaveChangesAsync();
            farmDto.Id = farm.Id;
            return farmDto;
        }

        public async Task<bool> UpdateFarm(int id, FarmDto farmDto)
        {
            if (id != farmDto.Id)
            {
                return false;
            }

            var farmInDb = await _context.Farms.FirstOrDefaultAsync(f => f.Id == id);
            if (farmInDb == null)
            {
                return false;
            }

            _mapper.Map(farmDto, farmInDb);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FarmExists(id))
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

        public async Task<bool> DeleteFarm(int id)
        {
            var farmInDb = await _context.Farms.FirstOrDefaultAsync(f => f.Id == id);
            if (farmInDb == null)
            {
                return false;
            }

            _context.Farms.Remove(farmInDb);
            await _context.SaveChangesAsync();
            return true;
        }

        private bool FarmExists(int id)
        {
            return _context.Farms.Any(e => e.Id == id);
        }
    }
}
