using Optional;

namespace StreamingVideoIndexer.Core.ValueObjects;

public class FileProperties
{
    public string Name { get; private set; }
    public string Path { get; private set; }
    public long Size { get; private set; }
    public Option<string> ThumbnailPath { get; private set; } = Option.None<string>();
    public TimeSpan Duration { get; private set; }
    public string? Description { get; private set; } = null;

    public FileProperties(string name, string path, long size, Option<string> thumbnailPath, TimeSpan duration)
    {
        Name = name;
        Path = path;
        Size = size;
        ThumbnailPath = thumbnailPath;
        Duration = duration;
    }

    public FileProperties(string name, string path, long size, Option<string> thumbnailPath, TimeSpan duration, string? description)
    {
        Name = name;
        Path = path;
        Size = size;
        ThumbnailPath = thumbnailPath;
        Duration = duration;
        Description = description;
    }
}
