using Microsoft.AspNetCore.Mvc;

namespace RF1.Services.PhotoClients
{
    public interface IPhotoService
    {
        public Task UploadPhotoAsync(IFormFile photo);
        public Task<byte[]> ReadPhotoAsync(string photoName);
        public Task DeletePhotoAsync(string photoName);
    }
}
