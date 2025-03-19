using StreamingVideoIndexer.Core.ValueObjects;
using System.Collections.Concurrent;

namespace StreamingVideoIndexer.Core.Interfaces.Services;

public interface IDirectoryWatcher
{
    void Start(ConcurrentQueue<FileProperties> filesToIndex);
    void Stop();
    void OnCreated(object sender, FileSystemEventArgs e);
}
