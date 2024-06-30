﻿using Microsoft.AspNetCore.Mvc;
using RF1.Dtos;
using RF1.Models;
using RF1.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RF1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingsService _ratingsService;

        public RatingsController(IRatingsService ratingsService)
        {
            _ratingsService = ratingsService;
        }

        // GET: api/Ratings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatings()
        {
            var ratings = await _ratingsService.GetRatings();
            return Ok(ratings);
        }

        // GET: api/Ratings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RatingDto>> GetRating(int id)
        {
            var rating = await _ratingsService.GetRating(id);
            if (rating == null)
            {
                return NotFound();
            }
            return Ok(rating);
        }

        // POST: api/Ratings
        [HttpPost]
        public async Task<ActionResult<RatingDto>> CreateRating(RatingDto ratingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdRating = await _ratingsService.CreateRating(ratingDto);
            return CreatedAtAction(nameof(GetRating), new { id = createdRating.Id }, createdRating);
        }

        // PUT: api/Ratings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(int id, RatingDto ratingDto)
        {
            if (id != ratingDto.Id)
            {
                return BadRequest("Mismatched IDs");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _ratingsService.UpdateRating(id, ratingDto);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Ratings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var result = await _ratingsService.DeleteRating(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
