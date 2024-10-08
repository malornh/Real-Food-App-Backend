﻿using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Dtos;
using RF1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RF1.Services
{
    public class RatingsService : IRatingsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public RatingsService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RatingDto>> GetRatings()
        {
            var ratings = await _context.Ratings.ToListAsync();

            return _mapper.Map<List<RatingDto>>(ratings);
        }

        public async Task<RatingDto> GetRating(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);

            return _mapper.Map<RatingDto>(rating);
        }

        public async Task<RatingDto> CreateRating(RatingDto ratingDto)
        {
            var rating = _mapper.Map<Rating>(ratingDto);

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            ratingDto.Id = rating.Id;

            return ratingDto;
        }
    }
}
