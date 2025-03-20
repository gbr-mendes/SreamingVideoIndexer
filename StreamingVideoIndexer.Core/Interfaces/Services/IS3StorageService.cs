namespace StreamingVideoIndexer.Core.Interfaces.Services;

public interface IS3StorageService
{
    Task<bool> UploadFileAsync(string fileName, string filePath);
}
