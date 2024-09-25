using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Models;
using RF1.Dtos;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;
using RF1.Services;

namespace RF1.Controllers.Api 
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FarmsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IFarmsService _farmService;

        public FarmsController(DataContext context, IMapper mapper, IFarmsService farmService)
        {
            _context = context;
            _mapper = mapper;
            _farmService = farmService;
        }

        // GET: api/Farms
        [HttpGet]
        public IEnumerable<FarmDto> GetFarms()
        {
            var farms = _context.Farms.ToList();
            return _mapper.Map<List<FarmDto>>(farms);
        }

        // GET: api/Farms/ByIds
        [HttpGet("ByIds")]
        public IEnumerable<FarmDto> GetFarmsByIds(string farmIds)
        {
            var farmIdsArray = farmIds.Split(',').Select(int.Parse).ToArray();
            var farms = _context.Farms.Where(f => farmIdsArray.Contains(f.Id)).ToList();
            return _mapper.Map<List<FarmDto>>(farms);
        }

        // GET: api/Farms/ByUser/5
        [HttpGet("ByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<FarmDto>>> GetFarmsByUserId(string userId)
        {
            var farms = await _context.Farms
                .Where(f => f.UserId == userId)
                .ToListAsync();

            if (farms == null || farms.Count == 0)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<List<FarmDto>>(farms));
        }


        // GET: api/Farms/5
        [HttpGet("{id}")]
        public ActionResult<FarmDto> GetFarm(int id)
        {
            var farm = _context.Farms.FirstOrDefault(f => f.Id == id);
            if (farm == null)
            {
                return NotFound();
            }
            return _mapper.Map<FarmDto>(farm);
        }

        // Method to get farm with products
        [HttpGet("{farmId}/FarmWithProducts")]
        public async Task<ActionResult<FarmFullInfoDto>> GetFarmWithProducts(int farmId)
        {
            // Fetch farm products using LINQ query
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
                    Quantity = p.Quantity.HasValue ? p.Quantity.Value : 0, // Handle nullable Quantity
                    DeliveryRadius = p.DeliveryRadius.HasValue ? p.DeliveryRadius.Value : 0, // Handle nullable DeliveryRadius
                    MinUnitOrder = p.MinUnitOrder,
                    DateUpdated = p.DateUpdated,
                    Rating = _context.Ratings
                            .Where(r => r.ProductId == p.Id)
                            .Average(r => r.RatingValue)
                })
                .ToListAsync();

            // Fetch farm details
            var farmDetails = await _context.Farms
                .Where(f => f.Id == farmId)
                .Select(f => new FarmFullInfoDto
                {
                    Id = f.Id,
                    PhotoId = f.PhotoId,
                    UserId = f.UserId,
                    Name = f.Name,
                    Description = f.Description,
                    Latitude = f.Latitude,
                    Longitude = f.Longitude,
                    Rating = f.Rating,
                    Products = farmProducts
                })
                .FirstOrDefaultAsync();

            // Check if any data is returned
            if (farmDetails == null)
            {
                return NotFound("No data found for the farm");
            }

            return Ok(farmDetails);
        }

        // POST: api/Farms
        [HttpPost]
        public async Task<ActionResult<FarmDto>> PostFarm(FarmDto farmDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            farmDto = await _farmService.CreateFarm(farmDto);

            return CreatedAtAction("GetFarm", new { id = farmDto.Id }, farmDto);
        }

        // PUT: api/Farms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFarm(int id, FarmDto farmDto)
        {
            if (id != farmDto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            farmDto = await _farmService.UpdateFarm(id, farmDto);

            return Ok(farmDto);
        }


        // DELETE: api/Farms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarm(int id)
        {
            await _farmService.DeleteFarm(id);
            return Ok();
        }

        private bool FarmExists(int id)
        {
            return _context.Farms.Any(e => e.Id == id);
        }

    }
}
