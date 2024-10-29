using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SWD392_BE.Services.Services
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string? imageName = default);

        string GetImageUrl(string imageName);

        Task<string> UpdateImageAsync(IFormFile imageFile, string imageName);

        Task DeleteImageAsync(string imageName);

        Task<string[]> UploadImagesAsync(IFormFileCollection files);
        string ExtractImageNameFromUrl(string imageUrl);
    }
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly IConfiguration _configuration;
        private readonly string _bucketName;

        public FirebaseStorageService(IConfiguration configuration, ILogger<FirebaseStorageService> logger)
        {
            _configuration = configuration;
            _bucketName = _configuration["Firebase:Bucket"]!;

            try
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                logger.LogInformation($"Current Environment: {environment}");

                GoogleCredential googleCredential;

                if (environment == Environments.Production)
                {
                    var base64JsonAuth = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS");
                    if (string.IsNullOrEmpty(base64JsonAuth))
                    {
                        throw new InvalidOperationException("FIREBASE_CREDENTIALS environment variable is not set.");
                    }

                    var jsonAuthBytes = Convert.FromBase64String(base64JsonAuth);
                    var jsonAuth = Encoding.UTF8.GetString(jsonAuthBytes);
                    googleCredential = GoogleCredential.FromJson(jsonAuth);
                }
                else
                {
                    var firebaseAuthPath = _configuration["Firebase:AuthFile"];
                    googleCredential = GoogleCredential.FromFile(firebaseAuthPath);
                }

                _storageClient = StorageClient.Create(googleCredential);
                logger.LogInformation("Google credentials successfully loaded.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error loading Google credentials: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteImageAsync(string imageName)
        {
            await _storageClient.DeleteObjectAsync(_bucketName, imageName, cancellationToken: CancellationToken.None);
        }

        public string GetImageUrl(string imageName)
        {
            string imageUrl = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(imageName)}?alt=media";
            return imageUrl;
        }

        public async Task<string> UpdateImageAsync(IFormFile imageFile, string imageName)
        {
            using var stream = new MemoryStream();
            await imageFile.CopyToAsync(stream);
            stream.Position = 0;

            var blob = await _storageClient.UploadObjectAsync(_bucketName, imageName, imageFile.ContentType, stream, cancellationToken: CancellationToken.None);
            return GetImageUrl(imageName);
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string? imageName = default)
        {
            imageName ??= $"{Path.GetFileNameWithoutExtension(imageFile.FileName)}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";

            using var stream = new MemoryStream();
            await imageFile.CopyToAsync(stream);

            var blob = await _storageClient.UploadObjectAsync(_bucketName, imageName, imageFile.ContentType, stream, cancellationToken: CancellationToken.None);

            if (blob is null)
            {
                throw new Exception("Upload image failed");
            }

            return GetImageUrl(imageName);
        }

        public async Task<string[]> UploadImagesAsync(IFormFileCollection files)
        {
            var uploadTasks = new List<Task<string>>();
            foreach (var file in files)
            {
                uploadTasks.Add(UploadImageAsync(file));
            }

            var imageUrls = await Task.WhenAll(uploadTasks);
            return imageUrls;
        }

        public string ExtractImageNameFromUrl(string imageUrl)
        {
            int start = imageUrl.IndexOf("o/") + 2;  // +2 to skip past 'o/'
            int end = imageUrl.IndexOf("?alt=media");
            string imageName = imageUrl.Substring(start, end - start);
            return imageName;
        }
    }
}
