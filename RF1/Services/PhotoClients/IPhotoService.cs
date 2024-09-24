using Microsoft.AspNetCore.Mvc;

namespace RF1.Services.PhotoClients
{
    public interface IPhotoService
    {
        public Task UploadPhotoAsync(IFormFile photo, string userId);
        public Task<IActionResult> ReadPhotoAsync(string photoName);
        public Task DeletePhotoAsync(string photoName);
    }
}
