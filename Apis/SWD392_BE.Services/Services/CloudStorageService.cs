using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SWD392_BE.Repositories.Utils.ConfigOptions;
using SWD392_BE.Services.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Services
{
    public class CloudStorageService : ICloudStorageService
    {
        private readonly GCSConfigOptions _options;
        private readonly ILogger<CloudStorageService> _logger;
        private readonly GoogleCredential _googleCredential;

        public CloudStorageService(IOptions<GCSConfigOptions> options, ILogger<CloudStorageService> logger)
        {
            _options = options.Value;
            _logger = logger;

            try
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                _logger.LogInformation($"Current Environment: {environment}");

                if (environment == Environments.Production)
                {
                    var base64JsonAuth = Environment.GetEnvironmentVariable("GCP_STORAGE_AUTH");
                    if (string.IsNullOrEmpty(base64JsonAuth))
                    {
                        throw new InvalidOperationException("GCP_STORAGE_AUTH environment variable is not set.");
                    }

                    var jsonAuthBytes = Convert.FromBase64String(base64JsonAuth);
                    var jsonAuth = System.Text.Encoding.UTF8.GetString(jsonAuthBytes);
                    _googleCredential = GoogleCredential.FromJson(jsonAuth);
                }
                else
                {
                    _googleCredential = GoogleCredential.FromFile(_options.GCPStorageAuthFile);
                }

                _logger.LogInformation("Google credentials successfully loaded.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading Google credentials: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteFileAsync(string fileNameToDelete)
        {
            try
            {
                using (var storageClient = StorageClient.Create(_googleCredential))
                {
                    await storageClient.DeleteObjectAsync(_options.GoogleCloudStorageBucketName, fileNameToDelete);
                }
                _logger.LogInformation($"File {fileNameToDelete} deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while deleting file {fileNameToDelete}: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetSignedUrlAsync(string fileNameToRead, int timeOutInMinutes = 30)
        {
            try
            {
                var sac = _googleCredential.UnderlyingCredential as ServiceAccountCredential;
                var urlSigner = UrlSigner.FromServiceAccountCredential(sac);
                var signedUrl = await urlSigner.SignAsync(_options.GoogleCloudStorageBucketName, fileNameToRead, TimeSpan.FromMinutes(timeOutInMinutes));
                _logger.LogInformation($"Signed url obtained for file {fileNameToRead}");
                return signedUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while obtaining signed url for file {fileNameToRead}: {ex.Message}");
                throw;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile fileToUpload, string fileNameToSave, int maxWidth, int maxHeight)
        {
            try
            {
                _logger.LogInformation($"Uploading: file {fileNameToSave} to storage {_options.GoogleCloudStorageBucketName}");

                using (var memoryStream = new MemoryStream())
                {
                    // Copy the uploaded file to memory stream
                    await fileToUpload.CopyToAsync(memoryStream);

                    // Use ImageSharp to resize the image
                    memoryStream.Position = 0;
                    using (var image = Image.Load(memoryStream))
                    {
                        // Resize image if it exceeds maxWidth or maxHeight
                        if (image.Width > maxWidth || image.Height > maxHeight)
                        {
                            image.Mutate(x => x.Resize(maxWidth, maxHeight));
                        }

                        // Save the resized image back to memory stream
                        memoryStream.Position = 0;
                        memoryStream.SetLength(0);
                        await image.SaveAsync(memoryStream, new PngEncoder()); // Use appropriate encoder based on your needs
                    }

                    // Upload the resized image to Google Cloud Storage
                    using (var storageClient = StorageClient.Create(_googleCredential))
                    {
                        var uploadedFile = await storageClient.UploadObjectAsync(
                            _options.GoogleCloudStorageBucketName,
                            fileNameToSave,
                            fileToUpload.ContentType,
                            memoryStream);
                        _logger.LogInformation($"Uploaded: file {fileNameToSave} to storage {_options.GoogleCloudStorageBucketName}");
                        return uploadedFile.MediaLink;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while uploading file {fileNameToSave}: {ex.Message}");
                throw;
            }
        }
    }
}
