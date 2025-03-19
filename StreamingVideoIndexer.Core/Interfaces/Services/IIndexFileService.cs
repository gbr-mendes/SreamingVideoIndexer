using StreamingVideoIndexer.Core.ValueObjects;

namespace StreamingVideoIndexer.Core.Interfaces.Services;

public interface IIndexFileService
{
    Task IndexFile(FileProperties fileProperties, bool reindex = false);
}
