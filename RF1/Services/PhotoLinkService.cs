using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
        public async Task CreatePhotoLinkAsync(PhotoLink photoLink)
        {
            await _context.PhotoLinks.AddAsync(photoLink);

            await _context.SaveChangesAsync();
        }

        public async Task<IQueryable> GetUserPhotoLinks(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            return _context.PhotoLinks.Where(p => p.UserId == user.Id).Select(u => u.Id);
        }

        [HttpPut("{id}")]
        public async Task<PhotoLink> UpdatePhotoLink(string id, PhotoLink photoLink)
        {
            var existingPhotoLink = await _context.PhotoLinks.FirstOrDefaultAsync(p => p.Id == id);
            if (existingPhotoLink == null) throw new ArgumentNullException("Photo not found");

            _context.PhotoLinks.Remove(existingPhotoLink);

            await _context.PhotoLinks.AddAsync(photoLink);
            await _context.SaveChangesAsync();

            return photoLink;
        }

        public async Task DeletePhotoLinkAsync(string photoId)
        {
            var photo = await _context.PhotoLinks.FirstOrDefaultAsync(p => p.Id == photoId);
            if (photo == null) throw new ArgumentNullException("Photo not found");

            _context.PhotoLinks.Remove(photo);
            _context.SaveChanges();
        }
    }
}
