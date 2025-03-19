using StreamingVideoIndexer.Core.Models;

namespace StreamingVideoIndexer.Core.Interfaces.Repositories;

public interface IIndexFilesRepository
{
    Task AddIndexedFileAsync(IndexedFile indexedFile);
}
