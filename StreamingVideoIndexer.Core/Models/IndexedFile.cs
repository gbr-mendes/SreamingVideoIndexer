namespace StreamingVideoIndexer.Core.Models;

public class IndexedFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Path { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailPath { get; set; }
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public TimeSpan Duration { get; set; }
}
