namespace RF1.Services.PhotoClients
{
    public interface IFreeImageHostService
    {
        public Task<string> StorePhotoAsync(IFormFile photo);

        public Task<string> UpdatePhotoAsync(IFormFile photo);

        public Task DeletePhotoAsync(string photoUrl);

    }
}
