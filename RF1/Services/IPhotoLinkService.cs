using RF1.Models;

namespace RF1.Services
{
    public interface IPhotoLinkService
    {
        Task<IQueryable> GetUserPhotoLinks(string userId);
        Task CreatePhotoLinkAsync(PhotoLink photoLink);
        Task<PhotoLink> UpdatePhotoLink(string id, PhotoLink photo);
        Task DeletePhotoLinkAsync(string photoId);
    }
}
