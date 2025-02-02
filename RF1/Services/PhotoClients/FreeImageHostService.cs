using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RF1.Services.UserAccessorService;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace RF1.Services.PhotoClients
{
    public class FreeImageHostService : IFreeImageHostService
    {
        private readonly HttpClient _httpClient;
        private readonly IUserAccessorService _userAccessorService;
        private const string ApiKey = "6d207e02198a847aa98d0a2a901485a5";
        private const string UploadUrl = "https://freeimage.host/api/1/upload";

        public FreeImageHostService(HttpClient httpClient, IUserAccessorService userAccessorService)
        {
            _httpClient = httpClient;
            _userAccessorService = userAccessorService;
        }

        public async Task<string> StorePhotoAsync(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                throw new ArgumentException("No file provided.");

            var userId = _userAccessorService.GetUserId();

            using var content = new MultipartFormDataContent();
            using var fileStream = photo.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(photo.ContentType);
            content.Add(fileContent, "source", photo.FileName);
            content.Add(new StringContent(ApiKey), "key");

            var response = await _httpClient.PostAsync(UploadUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to upload photo: {responseBody}");

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
            var imageUrl = jsonResponse.GetProperty("image").GetProperty("url").GetString();

            if (string.IsNullOrEmpty(imageUrl))
                throw new Exception("Image URL not found in response.");

            return imageUrl;
        }

        public async Task<string> UpdatePhotoAsync(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                throw new ArgumentException("No file provided.");

            // The client doesn't support deletion of photos, so we just upload a new one.
            return await StorePhotoAsync(photo);
        }

        public Task DeletePhotoAsync(string photoUrl)
        {
            // The client doesn't support deletion of photos, so we just upload a new one.
            return Task.CompletedTask;
        }
    }
}
