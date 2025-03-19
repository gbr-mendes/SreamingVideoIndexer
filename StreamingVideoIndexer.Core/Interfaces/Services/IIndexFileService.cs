using StreamingVideoIndexer.Core.ValueObjects;

namespace StreamingVideoIndexer.Core.Interfaces.Services;

public interface IIndexFileService
{
    void IndexFile(FileProperties fileProperties, bool reindex = false);
}
