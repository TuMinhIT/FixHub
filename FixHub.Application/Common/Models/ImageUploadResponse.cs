
namespace FixHub.Application.Common.Models
{
    public class ImageUploadResponse
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string Format { get; set; } = string.Empty;
    }
}
