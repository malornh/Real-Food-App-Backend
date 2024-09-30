using RF1.Dtos;
using RF1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RF1.Services
{
    public interface IRatingsService
    {
        Task<IEnumerable<RatingDto>> GetRatings();
        Task<RatingDto> GetRating(int id);
        Task<RatingDto> CreateRating(RatingDto ratingDto);
        Task<RatingDto> UpdateRating(int id, RatingDto ratingDto);
        Task<bool> DeleteRating(int id);
    }
}
