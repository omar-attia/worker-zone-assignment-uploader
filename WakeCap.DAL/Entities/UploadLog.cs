namespace WakeCap.DAL.Entities;

public class UploadLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = default!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int ErrorRows { get; set; }
    public string Status { get; set; } = default!; // "Saved" or "Rejected"
    public string? ErrorDetailsJson { get; set; }
}
