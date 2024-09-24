using RF1.Models;

namespace RF1.Services
{
    public interface IPhotoLinkService
    {
        IQueryable GetUserPhotoLinks(string userId);
        Task CreatePhotoLinkAsync(PhotoLink photoLink);
        Task<PhotoLink> UpdatePhotoLink(PhotoLink photo);
        Task DeletePhotoLinkAsync(string photoId);
    }
}
