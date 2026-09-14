namespace FixHub.Domain.Entities;

public class UploadedImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Url { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string Format { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
