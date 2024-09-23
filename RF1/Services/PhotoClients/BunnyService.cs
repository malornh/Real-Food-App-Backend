using RF1.Services.PhotoClients;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class BunnyService : IPhotoService
{
    private readonly HttpClient _httpClient;
    private string apiKey = "6892ff89-574b-4f1d-9286d9d4961a-da4d-4696";
    private string storageUrl = "https://storage.bunnycdn.com/real-food-app";

    public BunnyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task UploadPhotoAsync(IFormFile photo)
    {
        if (photo == null || photo.Length == 0)
        {
            throw new ArgumentException("No file provided.");
        }

        using (var content = new StreamContent(photo.OpenReadStream()))
        {
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            _httpClient.DefaultRequestHeaders.Add("AccessKey", apiKey);

            var response = await _httpClient.PutAsync($"{storageUrl}/{photo.FileName}", content); // THAT MUST BE THE USER'S PHOTO ID FROM DB
            response.EnsureSuccessStatusCode();
        }
    }

    public async Task<byte[]> ReadPhotoAsync(string fileName)
    {
        var url = $"{storageUrl}/{fileName}";
        _httpClient.DefaultRequestHeaders.Add("AccessKey", apiKey);

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task DeletePhotoAsync(string fileName)
    {
        var url = $"{storageUrl}/{fileName}";
        _httpClient.DefaultRequestHeaders.Add("AccessKey", apiKey);

        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
