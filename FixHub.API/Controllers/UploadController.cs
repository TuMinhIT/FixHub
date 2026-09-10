using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [Route("api/v1/uploads")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IImageStorageService _imageStorageService;

        public UploadController(IImageStorageService imageStorageService)
        {
            _imageStorageService = imageStorageService;
        }

        [HttpPost("image")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
        {
            var result = await _imageStorageService.UploadAsync(file, "fixhub/users", cancellationToken);

            return Ok(new ApiResponse<ImageUploadResponse>(result, "Upload successful"));
        }

        [HttpDelete("image/{publicId}")]
        [Authorize]
        public async Task<IActionResult> DeleteImage(string publicId, CancellationToken cancellationToken)
        {
            var deleted = await _imageStorageService.DeleteAsync(publicId, cancellationToken);
            return Ok(new ApiResponse<bool>(deleted, "Delete successful"));
        }
    }
}
