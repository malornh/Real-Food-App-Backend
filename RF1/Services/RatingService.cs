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

        public async Task<bool> UpdateRating(int id, RatingDto ratingDto)
        {
            if (id != ratingDto.Id)
            {
                return false;
            }

            var ratingInDb = await _context.Ratings.FirstOrDefaultAsync(r => r.Id == id);
            if (ratingInDb == null)
            {
                return false;
            }

            _mapper.Map(ratingDto, ratingInDb);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(id))
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

        public async Task<bool> DeleteRating(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return false;
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return true;
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }
    }
}
