namespace FixHub.Domain.Entities;

public class RagFeedback
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public string Question { get; set; } = string.Empty;
    public bool WasHelpful { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
