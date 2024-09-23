using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RF1.Services.PhotoClients;
using System.IO;
using System.Threading.Tasks;

namespace RF1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoController : ControllerBase
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly IPhotoService _photoService;

        public PhotoController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            try
            {
                await _photoService.UploadPhotoAsync(photo);
                return Ok(new { message = "Image uploaded successfully!"});
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading image: {ex.Message}");
            }
        }

        [HttpGet("read/{fileName}")]
        public async Task<IActionResult> DownloadImage(string fileName)
        {
            try
            {
                var photo = await _photoService.ReadPhotoAsync(fileName);

                return photo;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error uploading image: {ex.Message}", ex);
            }
        }

        [HttpGet("delete/{fileName}")]
        public async Task<IActionResult> DeleteImage(string fileName)
        {
            try
            {
                var photo = await _photoService.ReadPhotoAsync(fileName);

                return photo;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error uploading image: {ex.Message}", ex);
            }
        }
    }
}
