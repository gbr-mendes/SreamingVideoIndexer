using StreamingVideoIndexer.Core.ValueObjects;

namespace StreamingVideoIndexer.Core.Interfaces.Services;

public interface IHandleFileService
{
    bool CreateSymbolicLink(string sourceFilePath, string destinationFilePath);
    Task<FileProperties> GetFileProperties(string filePath, bool isDirectory = false);

}
