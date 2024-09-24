using RF1.Models;

namespace RF1.Services
{
    public interface IPhotoLinkService
    {
        IQueryable GetUserPhotoLinks(string userId);
        Task<PhotoLink> CreatePhotoLink(PhotoLink photoLink);
        //Task<int> UpdatePhotoLink(string userId);
        //Task DeletePhotoLink(int photoId);
    }
}
