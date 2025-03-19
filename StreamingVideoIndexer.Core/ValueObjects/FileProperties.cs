using Optional;

namespace StreamingVideoIndexer.Core.ValueObjects;

public class FileProperties
{
    public string Path { get; private set; }
    public long Size { get; private set; }
    public Option<string> ThumbnailPath { get; private set; } = Option.None<string>();

    public FileProperties(string path, long size, Option<string> thumbnailPath)
    {
        Path = path;
        Size = size;
        ThumbnailPath = thumbnailPath;
    }
}
