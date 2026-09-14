namespace FixHub.Application.Common.Models;

public sealed class UploadedImageResponse
{
    public Guid Id { get; init; }
    public string Url { get; init; } = string.Empty;
    public string PublicId { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public string Format { get; init; } = string.Empty;
    public Guid UploadedByUserId { get; init; }
    public string UploadedByEmail { get; init; } = string.Empty;
    public DateTime? UploadedAt { get; init; }
    public bool IsInUse { get; init; }
}
