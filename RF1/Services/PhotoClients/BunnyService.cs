using Microsoft.AspNetCore.Mvc;
using RF1.Models;
using RF1.Services;
using RF1.Services.PhotoClients;
using RF1.Services.UserAccessorService;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class BunnyService : IPhotoService
{
    private readonly HttpClient _httpClient;
    private string apiKey;
    private string storageUrl = "https://storage.bunnycdn.com/real-food-app";
    private readonly IPhotoLinkService _photoLinkService;
    private readonly IUserAccessorService _userAccessorService;

    public BunnyService(HttpClient httpClient,
        IPhotoLinkService photoLinkService,
        IConfiguration configuration,
        IUserAccessorService userAccessorService)
    {
        _httpClient = httpClient;
        _photoLinkService = photoLinkService;
        apiKey = configuration["BUNNY_API_KEY"];
        _userAccessorService = userAccessorService;

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("Bunny API Key not found in User Secrets.");
        }
    }

    public async Task<string> StorePhotoAsync(IFormFile photo)
    {
        if (photo == null || photo.Length == 0)
        {
            throw new ArgumentException("No file provided.");
        }
        
         var userId = _userAccessorService.GetUserId();

        var photoExtension = Path.GetExtension(photo.FileName);
        var newPhotoLink = new PhotoLink
        {
            Id = Guid.NewGuid().ToString() + photoExtension,
            UserId = userId
        };

        await _photoLinkService.CreatePhotoLinkAsync(newPhotoLink);

        using (var content = new StreamContent(photo.OpenReadStream()))
        {
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

            _httpClient.DefaultRequestHeaders.Remove("AccessKey");
            _httpClient.DefaultRequestHeaders.Add("AccessKey", apiKey);

            var response = await _httpClient.PutAsync($"{storageUrl}/{newPhotoLink.Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to upload photo to cloud: {errorMessage}");
            }
            return newPhotoLink.Id;
        }
    }

    public async Task<IActionResult> ReadPhotoAsync(string fileName)
    {
        var url = $"{storageUrl}/{fileName}";
        _httpClient.DefaultRequestHeaders.Remove("AccessKey");
        _httpClient.DefaultRequestHeaders.Add("AccessKey", apiKey);

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var imageData = await response.Content.ReadAsByteArrayAsync();
        var contentType = GetContentType(fileName);

        return new FileContentResult(imageData, contentType)
        {
            FileDownloadName = fileName
        };
    }

    public async Task<string> UpdatePhotoAsync(IFormFile photo, string fileName)
    {
        if (photo == null || photo.Length == 0)
        {
            throw new ArgumentException("No file provided.");
        }

        var userId = _userAccessorService.GetUserId();

        await DeletePhotoAsync(fileName);

        var photoId = await StorePhotoAsync(photo);

        return photoId;
    }

    public async Task DeletePhotoAsync(string fileName)
    {
        var url = $"{storageUrl}/{fileName}";
        _httpClient.DefaultRequestHeaders.Add("AccessKey", apiKey);

        // TO-DO: We must delete from db, delete from cloud and then we dont get any errors _context.SaveChanges(); 
        // Same for Storing Photos

        // Delete from cloud
        var response = await _httpClient.DeleteAsync(url);

        // Delete from db
        await _photoLinkService.DeletePhotoLinkAsync(fileName);

        response.EnsureSuccessStatusCode();
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".tiff" => "image/tiff",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
