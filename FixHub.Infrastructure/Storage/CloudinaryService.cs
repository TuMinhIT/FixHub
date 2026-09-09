using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FixHub.Infrastructure.Storage
{
    public class CloudinaryService : IImageStorageService
    {
        private const long MaxFileSizeBytes = 10 * 1024 * 1024;
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary configuration is missing.");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResponse> UploadAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
            {
                throw new BadRequestException("File is required.");
            }

            if (file.Length > MaxFileSizeBytes)
                throw new BadRequestException("Image file must not exceed 10 MB.");

            if (!IsImage(file.ContentType))
            {
                throw new BadRequestException("Only image files are allowed.");
            }

            var extension = Path.GetExtension(file.FileName);
            if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }
                .Contains(extension, StringComparer.OrdinalIgnoreCase))
                throw new BadRequestException("Unsupported image format.");
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var publicId = $"{folderName}/{Path.GetFileNameWithoutExtension(fileName)}";

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                PublicId = publicId,
                Folder = folderName,
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

            if (result.Error != null)
            {
                throw new ServiceUnavailableException("Image storage provider is unavailable.");
            }

            return new ImageUploadResponse
            {
                Url = result.SecureUrl?.ToString() ?? result.Url?.ToString() ?? string.Empty,
                PublicId = result.PublicId,
                FileName = file.FileName,
                SizeBytes = file.Length,
                Format = result.Format ?? Path.GetExtension(file.FileName).TrimStart('.')
            };
        }

        public async Task<bool> DeleteAsync(string publicId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                return false;
            }

            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            return result.Result == "ok";
        }

        private static bool IsImage(string contentType)
        {
            return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }
    }
}
