using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Models;

namespace RF1.Services
{
    public class PhotoLinkService : IPhotoLinkService
    {
        private readonly DataContext _context;

        public PhotoLinkService(DataContext context)
        {
            _context = context;
        }

        public IQueryable GetUserPhotoLinks(string userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            return _context.PhotoLinks.Where(p => p.UserId == user.Id).Select(u => u.Id);
        }

        public async Task<PhotoLink> CreatePhotoLink(PhotoLink photoLink)
        {
            await _context.PhotoLinks.AddAsync(photoLink);
            await _context.SaveChangesAsync();

            return photoLink;
        }

    }
}
