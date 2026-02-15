using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;


namespace LearningAzureScalableAPI.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<BlobStorageService> _logger;
        //private readonly string _connectionString;
        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            _logger = logger;
            var containerName = configuration["BlobContainerName"];
            //_connectionString = configuration["learning-azure-connectionstring"];
            _logger.LogInformation("ContainerName: {containerName}", containerName);
            //_logger.LogInformation("ConnectionString: {connectionString}", _connectionString);
            BlobServiceClient blobServiceClient =
                new BlobServiceClient(new Uri($"https://storagelearningazureblob.blob.core.windows.net"),
                new DefaultAzureCredential()
            );

            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }
        public async Task<string> UploadAsync(IFormFile file, string userId, string category)
        {
            var blobPath = $"users/{userId}/{category}/{file.FileName}";
            _logger.LogInformation("Uploading file {file} for user {user}", file.FileName, userId);
            var blobClient = _containerClient.GetBlobClient(blobPath);
            try
            {

                var metadata = new Dictionary<string, string>
            {
                { "uploadedBy",userId},
                { "category",category },
                { "uploadedOn",DateTime.UtcNow.ToString("yyyy-MM-dd")}
            };
                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    Metadata = metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blob upload failed");
                throw;   // VERY IMPORTANT
            }

            return blobClient.Uri.ToString();
        }

        public async Task<(byte[] data, string contentType)> DownloadAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var download = await blobClient.DownloadAsync();
            using var ms = new MemoryStream();
            await download.Value.Content.CopyToAsync(ms);
            return (ms.ToArray(), download.Value.Details.ContentType);
        }
        public string GetReadSasUrl(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerClient.Name,
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        public async Task<List<string>> ListFilesAsync()
        {
            var files = new List<string>();

            await foreach (var blob in _containerClient.GetBlobsAsync())
            {
                files.Add(blob.Name);
            }

            return files;
        }
        public async Task<bool> DeleteAsync(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                var response = await blobClient.DeleteIfExistsAsync();
                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "file delete failed");
                return false;
            }
        }

        //public Task<string> GetSecretAsync(string secret)
        //{
        //    return Task.FromResult(_connectionString);

        //}
    }
}
