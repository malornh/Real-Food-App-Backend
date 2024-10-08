﻿using Microsoft.AspNetCore.Mvc;

namespace RF1.Services.PhotoClients
{
    public interface IPhotoService
    {
        public Task<string> StorePhotoAsync(IFormFile photo);
        public Task<IActionResult> ReadPhotoAsync(string photoName);
        public Task<string> UpdatePhotoAsync(IFormFile photo, string photoName);
        public Task DeletePhotoAsync(string photoName);
    }
}
