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
        public async Task<ActionResult<IEnumerable<FarmDto>>> GetFarms()
        {
            var farms = await _farmService.GetAllFarmsAsync();

            if (farms == null || !farms.Any())
            {
                return NotFound();
            }

            return Ok(farms);
        }

        // GET: api/Farms/ByIds
        [HttpGet("ByIds")]
        public async Task<ActionResult<IEnumerable<FarmDto>>> GetFarmsByIds(string farmIds)
        {
            var farms = await _farmService.GetFarmsByIdsAsync(farmIds);

            if (farms == null || !farms.Any())
            {
                return NotFound();
            }

            return Ok(farms);
        }

        // GET: api/Farms/ByUser
        [Authorize]
        [HttpGet("UserFarms")]
        public async Task<ActionResult<IEnumerable<FarmDto>>> GetFarmsByUserId()
        {
            var farms = await _farmService.GetFarmsByUserIdAsync();

            if (farms == null || !farms.Any())
            {
                return NotFound();
            }

            return Ok(farms);
        }

        // GET: api/Farms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FarmDto>> GetFarm(int id)
        {
            var farm = await _farmService.GetFarmByIdAsync(id);

            if (farm == null)
            {
                return NotFound();
            }

            return Ok(farm);
        }

        // GET: api/Farms/5/FarmWithProducts
        [HttpGet("{farmId}/FarmWithProducts")]
        public async Task<ActionResult<FarmFullInfoDto>> GetFarmWithProducts(int farmId)
        {
            var farmWithProducts = await _farmService.GetFarmWithProductsAsync(farmId);

            if (farmWithProducts == null)
            {
                return NotFound("No data found for the farm");
            }

            return Ok(farmWithProducts);
        }

        // POST: api/Farms
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<FarmDto>> CreateFarm(FarmDto farmDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            farmDto = await _farmService.CreateFarm(farmDto);

            return CreatedAtAction("GetFarm", new { id = farmDto.Id }, farmDto);
        }

        // PUT: api/Farms/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<FarmDto>> UpdateFarm(int id, FarmDto farmDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            farmDto = await _farmService.UpdateFarm(id, farmDto);

            return Ok(farmDto);
        }


        // DELETE: api/Farms/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarm(int id)
        {
            await _farmService.DeleteFarm(id);

            return NoContent();
        }
    }
}
