using StreamingVideoIndexer.Shared.Interfaces;
using System.Security.Cryptography;

namespace StreamingVideoIndexer.Shared.Services;

public class HasherService : IHasherService
{
    public string CalculateMd5(string filePath, int bufferSize = 8192)
    {
        using var md5 = MD5.Create();
        using var fileStream = File.OpenRead(filePath);

        var buffer = new byte[bufferSize];
        int bytesRead;

        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            md5.TransformBlock(buffer, 0, bytesRead, null, 0);
        }

        md5.TransformFinalBlock(buffer, 0, 0);

        return BitConverter.ToString(md5.Hash).Replace("-", "").ToLowerInvariant();
    }
}
