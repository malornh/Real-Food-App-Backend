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

namespace RF1.Controllers.Api 
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FarmsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FarmsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        // POST: api/Farms
        [HttpPost]
        public IActionResult CreateFarm(FarmDto farmDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var farm = _mapper.Map<Farm>(farmDto);

            _context.Farms.Add(farm);
            _context.SaveChanges();

            farmDto.Id = farm.Id;

            return Created(new Uri(Request.GetDisplayUrl() + "/" + farmDto.Id), farmDto);
        }

        // PUT: api/Farms/5
        [HttpPut("{id}")]
        public IActionResult UpdateFarm(int id, FarmDto farmDto)
        {
            if (id != farmDto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var farmInDb = _context.Farms.FirstOrDefault(f => f.Id == id);
            if (farmInDb == null)
            {
                return NotFound();
            }

            _mapper.Map(farmDto, farmInDb);

            _context.SaveChanges();

            return Ok(farmDto);
        }

        // DELETE: api/Farms/5
        [HttpDelete("{id}")]
        public IActionResult DeleteFarm(int id)
        {
            var farmInDb = _context.Farms.FirstOrDefault(f => f.Id == id);
            if (farmInDb == null)
            {
                return NotFound();
            }

            _context.Farms.Remove(farmInDb);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
