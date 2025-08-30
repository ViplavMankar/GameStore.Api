using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameStore.Api.Services;

namespace GameStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IS3Service _s3Service;

        public FilesController(IS3Service s3Service)
        {
            _s3Service = s3Service;
        }

        [HttpPost("upload-zip")]
        public async Task<IActionResult> UploadZip(IFormFile zipFile, [FromForm] string gameName)
        {
            if (zipFile == null || zipFile.Length == 0)
                return BadRequest("No file uploaded.");

            if (string.IsNullOrWhiteSpace(gameName))
                return BadRequest("Game name is required.");

            using var zipStream = zipFile.OpenReadStream();
            var mainFileUrl = await _s3Service.UploadZipAsync(zipStream, gameName);

            if (string.IsNullOrEmpty(mainFileUrl))
                return Ok(new { Url = "No HTML file found in ZIP" });

            return Ok(new { Url = mainFileUrl });
        }

        [HttpPost("upload-thumbnail")]
        public async Task<IActionResult> UploadThumbnail(IFormFile file, [FromForm] string gameName)
        {
            Console.WriteLine($"Received ContentType: {file.ContentType}");
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (string.IsNullOrWhiteSpace(gameName))
                return BadRequest("Game name is required.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return BadRequest("Only JPEG, PNG, or GIF images are allowed.");

            var key = $"{gameName}/thumbnails/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            using var fileStream = file.OpenReadStream();
            var thumbnailUrl = await _s3Service.UploadFileAsync(fileStream, key, file.ContentType);

            return Ok(new { Url = thumbnailUrl });
        }
    }
}
