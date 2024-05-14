using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Models;
using RF1.Dtos;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;

namespace RF1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public RatingsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Ratings
        [HttpGet]
        public IEnumerable<RatingDto> GetRatings()
        {
            var ratings = _context.Ratings.ToList();
            return _mapper.Map<List<RatingDto>>(ratings);
        }

        // GET: api/Ratings/5
        [HttpGet("{id}")]
        public ActionResult<RatingDto> GetRating(int id)
        {
            var rating = _context.Ratings.FirstOrDefault(r => r.Id == id);
            if (rating == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<RatingDto>(rating));
        }

        // POST: api/Ratings
        [HttpPost]
        public IActionResult CreateRating(RatingDto ratingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rating = _mapper.Map<Rating>(ratingDto);

            _context.Ratings.Add(rating);
            _context.SaveChanges();

            ratingDto.Id = rating.Id;

            return Created(new Uri(Request.GetDisplayUrl() + "/" + ratingDto.Id), ratingDto);
        }

        // PUT: api/Ratings/5
        [HttpPut("{id}")]
        public IActionResult UpdateRating(int id, RatingDto ratingDto)
        {
            if (id != ratingDto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ratingInDb = _context.Ratings.FirstOrDefault(r => r.Id == id);
            if (ratingInDb == null)
            {
                return NotFound();
            }

            _mapper.Map(ratingDto, ratingInDb);

            _context.SaveChanges();

            return Ok(ratingDto);
        }

        // DELETE: api/Ratings/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRating(int id)
        {
            var ratingInDb = _context.Ratings.FirstOrDefault(r => r.Id == id);
            if (ratingInDb == null)
            {
                return NotFound();
            }

            _context.Ratings.Remove(ratingInDb);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
