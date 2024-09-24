﻿using Microsoft.AspNetCore.Mvc;
using RF1.Models;
using RF1.Services;
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
    private readonly IPhotoLinkService _photoLinkService;

    public BunnyService(HttpClient httpClient, IPhotoLinkService photoLinkService)
    {
        _httpClient = httpClient;
        _photoLinkService = photoLinkService;
    }

    public async Task UploadPhotoAsync(IFormFile photo, string userId)
    {
        if (photo == null || photo.Length == 0)
        {
            throw new ArgumentException("No file provided.");
        }

        using (var content = new StreamContent(photo.OpenReadStream()))
        {
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            _httpClient.DefaultRequestHeaders.Add("AccessKey", apiKey);

            var photoExtension = Path.GetExtension(photo.FileName);

            var photoLink = new PhotoLink
            {
                Id =  Guid.NewGuid().ToString() + photoExtension,
                UserId = userId
            };

            var createdPhotoLink = await _photoLinkService.CreatePhotoLink(photoLink);

            var response = await _httpClient.PutAsync($"{storageUrl}/{createdPhotoLink.Id}{photoExtension}", content); // THAT MUST BE THE USER'S PHOTO ID FROM DB
            response.EnsureSuccessStatusCode();
        }
    }

    public async Task<IActionResult> ReadPhotoAsync(string fileName)
    {
        var url = $"{storageUrl}/{fileName}";
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

    public async Task DeletePhotoAsync(string fileName)
    {
        var url = $"{storageUrl}/{fileName}";
        _httpClient.DefaultRequestHeaders.Add("AccessKey", apiKey);

        var response = await _httpClient.DeleteAsync(url);
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