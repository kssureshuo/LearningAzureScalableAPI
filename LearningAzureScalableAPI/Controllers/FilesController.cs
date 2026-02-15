using LearningAzureScalableAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningAzureScalableAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly BlobStorageService _blobStorageService;
        private readonly ILogger<FilesController> _logger;
        public FilesController(BlobStorageService blobStorageService, ILogger<FilesController> logger)
        {
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> Upload(IFormFile file, string userId, string category)
        {
            if (file == null || file.Length == 0)
                return BadRequest("NO File Uploaded!!!");
            var url = await _blobStorageService.UploadAsync(file, userId, category);
            return Ok(new { fileUrl = url });
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            var result = await _blobStorageService.DownloadAsync(fileName);
            return File(result.data, result.contentType, fileName);
        }

        [HttpGet("download-link/{fileName}")]
        public IActionResult GetDownloadLink(string fileName)
        {
            var url = _blobStorageService.GetReadSasUrl(fileName);
            return Ok(url);
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFiles()
        {
            var files = await _blobStorageService.ListFilesAsync();
            return Ok(files);
        }

        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
            var deleted = await _blobStorageService.DeleteAsync(fileName);

            if (!deleted)
                return NotFound("File not found");

            return Ok("File deleted");
        }

        [HttpGet("get-secret")]
        public async Task<IActionResult> GetSecret()
        {
            try
            {
                var value = await _blobStorageService.GetSecretAsync("MyAppSecret");
                _logger.LogInformation("app secret is: ", value);
                return Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError("Eror in GEtSEcert: ", ex);
                throw;
            }
        }
    }
}
