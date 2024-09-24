using RF1.Models;

namespace RF1.Services
{
    public interface IPhotoLinkService
    {
        IQueryable GetUserPhotoLinks(string userId);
        Task<PhotoLink> CreatePhotoLinkAsync(PhotoLink photoLink);
        //Task<int> UpdatePhotoLink(string userId);
        Task DeletePhotoLinkAsync(string photoId);
    }
}
