using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventEase_Management.Service
{
    public class ImageService
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };
        private const long MaxFileSize = 2 * 1024 * 1024; 
        private readonly string _blobConnectionString;
        private readonly string _blobContainerName;


        public ImageService(IConfiguration configuration)
        {
            _blobConnectionString = configuration.GetConnectionString("AzureBlobStorage");
            _blobContainerName = configuration["BlobContainerName"];
            //System.Diagnostics.Debug.WriteLine($"Using blob connection string: {_blobConnectionString}");
           //System.Diagnostics.Debug.WriteLine($"Using blob container name: {_blobContainerName}");

        }
        public async Task<string> UploadImageToBlobStorage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded.");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileMimeType = file.ContentType;

            if (!_allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Invalid file extension.");
            }

            if (!_allowedMimeTypes.Contains(fileMimeType))
            {
                throw new ArgumentException("Invalid file mime type.");
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException("File size exceeds the allowed limit of 2 MB.");
            }

            var blobServiceClient = new BlobServiceClient(_blobConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_blobContainerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var fileName = Guid.NewGuid().ToString() + fileExtension;
            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = fileMimeType });
            }

            return blobClient.Uri.ToString(); // Return the URL of the uploaded image
        }
        public async Task DeleteImageFromBlobStorage(string imageName)
        {
            var blobServiceClient = new BlobServiceClient(_blobConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_blobContainerName);
            var blobClient = containerClient.GetBlobClient(imageName);

            await blobClient.DeleteIfExistsAsync();
        }

        public string GenerateBlobSasUrl(string blobUrl, TimeSpan expiryTime)
        {
            var blobUri = new Uri(blobUrl);
            var blobContainerName = blobUri.Segments[1].Trim('/');
            var blobName = string.Join("", blobUri.Segments.Skip(2)).Trim('/');

            var blobServiceClient = new BlobServiceClient(_blobConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobContainerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }
    }
}
    