using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RF1.Services.PhotoClients;
using System.IO;
using System.Threading.Tasks;

namespace RF1.Controllers.Api
{

    // That controller is for TEST purposes only // To be deleted l8r

    [ApiController]
    [Route("api/[controller]")]
    public class BunnyController : ControllerBase
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private  readonly IPhotoService _photoService;

        public BunnyController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto([FromForm]IFormFile photo, [FromForm] string userId)
        {
            try
            {
                var photoId = await _photoService.StorePhotoAsync(photo);
                return Ok(new { message = $"Image created successfully! {photoId}" });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading image: {ex.Message}");
            }
        }

        [HttpGet("read/{fileName}")]
        public async Task<IActionResult> ReadPhoto(string fileName)
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

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePhoto([FromForm] IFormFile photo, [FromForm] string fileName, [FromForm] string userId)
        {
            try
            {
                var photoId = await _photoService.UpdatePhotoAsync(photo, fileName);
                return Ok(new { message = $"Image updated successfully! {photoId}" });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading image: {ex.Message}");
            }
        }

        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeletePhoto(string fileName)
        {
            try
            {
                await _photoService.DeletePhotoAsync(fileName);

                return Ok();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting image: {ex.Message}", ex);
            }
        }
    }
}
