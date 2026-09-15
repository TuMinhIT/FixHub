using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Models;
using FixHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixHub.API.Controllers
{
    [Route("api/v1/uploads")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IImageStorageService _imageStorageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UploadController(
            IImageStorageService imageStorageService,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _imageStorageService = imageStorageService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        [HttpPost("image")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var result = await _imageStorageService.UploadAsync(
                file,
                $"fixhub/users/{userId:N}",
                cancellationToken);

            var uploadedImage = new UploadedImage
            {
                Url = result.Url,
                PublicId = result.PublicId,
                FileName = result.FileName,
                SizeBytes = result.SizeBytes,
                Format = result.Format,
                UserId = userId,
                UploadedAt = DateTime.UtcNow
            };

            try
            {
                await _unitOfWork.UploadedImageRepository.AddAsync(uploadedImage);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                try
                {
                    await _imageStorageService.DeleteAsync(result.PublicId, CancellationToken.None);
                }
                catch
                {
                    // Keep the original database exception.
                }

                throw;
            }

            result.Id = uploadedImage.Id;

            return Ok(new ApiResponse<ImageUploadResponse>(result, "Upload successful"));
        }

        [HttpGet("images")]
        [Authorize]
        public async Task<IActionResult> GetUploadedImages(CancellationToken cancellationToken)
        {
            var isAdmin = User.IsInRole("Admin");
            var currentUserId = _currentUserService.UserId;
            var query = _unitOfWork.UploadedImageRepository.GetAll();
            if (!isAdmin)
            {
                query = query.Where(image => image.UserId == currentUserId);
            }

            var images = await query
                .OrderByDescending(image => image.UploadedAt)
                .ToListAsync(cancellationToken);
            var userIds = images.Select(image => image.UserId).Distinct().ToList();
            var uploaderEmails = await _unitOfWork.UserRepository.GetAll()
                .Where(user => userIds.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id, user => user.Email, cancellationToken);

            var publicIds = images.Select(image => image.PublicId).Distinct().ToList();
            var urls = images.Select(image => image.Url).Distinct().ToList();
            var linkedProductImages = await _unitOfWork.ProductImageRepository.GetAll()
                .Where(image =>
                    (image.PublicId != null && publicIds.Contains(image.PublicId)) ||
                    urls.Contains(image.ImageUrl))
                .Select(image => new { image.PublicId, image.ImageUrl })
                .ToListAsync(cancellationToken);
            var usedPublicIds = linkedProductImages
                .Where(image => image.PublicId != null)
                .Select(image => image.PublicId!)
                .ToHashSet(StringComparer.Ordinal);
            var usedUrls = linkedProductImages
                .Select(image => image.ImageUrl)
                .ToHashSet(StringComparer.Ordinal);

            var response = images.Select(image => new UploadedImageResponse
            {
                Id = image.Id,
                Url = image.Url,
                PublicId = image.PublicId,
                FileName = image.FileName,
                SizeBytes = image.SizeBytes,
                Format = image.Format,
                UploadedByUserId = image.UserId,
                UploadedByEmail = uploaderEmails.GetValueOrDefault(image.UserId, string.Empty),
                UploadedAt = image.UploadedAt,
                IsInUse = usedPublicIds.Contains(image.PublicId) || usedUrls.Contains(image.Url)
            }).ToList();

            if (isAdmin)
            {
                var trackedPublicIds = publicIds.ToHashSet(StringComparer.Ordinal);
                var trackedUrls = urls.ToHashSet(StringComparer.Ordinal);
                var legacyProductImages = await _unitOfWork.ProductImageRepository.GetAll()
                    .ToListAsync(cancellationToken);

                response.AddRange(legacyProductImages
                    .Where(image =>
                        !trackedUrls.Contains(image.ImageUrl) &&
                        (image.PublicId == null || !trackedPublicIds.Contains(image.PublicId)))
                    .GroupBy(image => image.PublicId ?? image.ImageUrl, StringComparer.Ordinal)
                    .Select(group => group.First())
                    .Select(image =>
                    {
                        var fileName = Uri.TryCreate(image.ImageUrl, UriKind.Absolute, out var uri)
                            ? Path.GetFileName(uri.AbsolutePath)
                            : Path.GetFileName(image.ImageUrl);

                        return new UploadedImageResponse
                        {
                            Id = image.Id,
                            Url = image.ImageUrl,
                            PublicId = image.PublicId ?? string.Empty,
                            FileName = string.IsNullOrWhiteSpace(fileName) ? image.ImageUrl : fileName,
                            Format = Path.GetExtension(fileName).TrimStart('.'),
                            UploadedByUserId = Guid.Empty,
                            UploadedByEmail = "Không lưu người tải lên (dữ liệu cũ)",
                            UploadedAt = null,
                            IsInUse = true
                        };
                    }));
            }

            return Ok(new ApiResponse<List<UploadedImageResponse>>(response));
        }

        [HttpDelete("images/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteImage(Guid id, CancellationToken cancellationToken)
        {
            var isAdmin = User.IsInRole("Admin");
            var currentUserId = _currentUserService.UserId;
            var image = await _unitOfWork.UploadedImageRepository
                .Find(record => record.Id == id && (isAdmin || record.UserId == currentUserId))
                .FirstOrDefaultAsync(cancellationToken);
            if (image == null)
                throw new NotFoundException(nameof(UploadedImage), id);

            var isUsedByProduct = await _unitOfWork.ProductImageRepository.GetAll()
                .AnyAsync(
                    productImage => productImage.PublicId == image.PublicId || productImage.ImageUrl == image.Url,
                    cancellationToken);
            if (isUsedByProduct)
                throw new ConflictException("Ảnh đang được sử dụng trong sản phẩm và không thể xóa.");

            var deletedFromStorage = await _imageStorageService.DeleteAsync(image.PublicId, cancellationToken);
            if (!deletedFromStorage)
                throw new ServiceUnavailableException("Không thể xóa ảnh khỏi kho lưu trữ.");

            await _unitOfWork.UploadedImageRepository.DeleteAsync(image.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Ok(new ApiResponse<bool>(true, "Image deleted successfully"));
        }
    }
}
