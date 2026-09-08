
using FixHub.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace FixHub.Application.Common.Interfaces
{
    public interface IImageStorageService
    {
        Task<ImageUploadResponse> UploadAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string publicId, CancellationToken cancellationToken = default);
    }
}