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

        [HttpPost("upload-docx")]
        public async Task<IActionResult> UploadDocx(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);

            var inputKey = $"documents/{fileId}/input{extension}";
            var outputKey = $"documents/{fileId}/output.pdf";

            // ✅ Upload DOCX to S3
            using var stream = file.OpenReadStream();
            await _s3Service.UploadFileAsync(stream, inputKey, file.ContentType);

            // ✅ Trigger conversion
            var pdfUrl = await _s3Service.ConvertDocxToPdf(fileId, inputKey, outputKey);

            return Ok(new
            {
                FileId = fileId,
                PdfUrl = pdfUrl
            });
        }

        [HttpPost("jpg-to-pdf")]
        [RequestSizeLimit(10_000_000)] // 10MB
        public async Task<IActionResult> ConvertJpgToPdf(IFormFile file)
        {
            // ❌ No file
            if (file == null)
                return BadRequest("No file uploaded.");

            // ❌ Reject multiple files (important)
            if (Request.Form.Files.Count > 1)
                return BadRequest("Only one file is allowed per request.");

            // ❌ Validate type
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Only image files are allowed.");

            // ❌ Strict JPG check
            if (!file.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                !file.FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only JPG/JPEG files are allowed.");
            }

            byte[] imageBytes;

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            var pdfBytes = _s3Service.ConvertSingleImageToPdf(imageBytes);

            return File(pdfBytes, "application/pdf", "converted.pdf");
        }
    }
}
