namespace StreamingVideoIndexer.Shared.Interfaces;

public interface IHasherService
{
    public string CalculateMd5(string filePath, int bufferSize = 8192);
}
