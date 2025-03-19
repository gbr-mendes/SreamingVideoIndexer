using Microsoft.Extensions.Logging;
using Optional.Collections;
using Optional;
using StreamingVideoIndexer.Core.Interfaces.Services;
using StreamingVideoIndexer.Core.ValueObjects;
using Optional.Unsafe;
using StreamingVideoIndexer.Core.Exceptions;

namespace StreamingVideoIndexer.Core.Services;

public class HandleFileService : IHandleFileService
{
    private readonly ILogger<HandleFileService> _logger;
    public HandleFileService(ILogger<HandleFileService> logger)
    {
        _logger = logger;
    }

    public FileProperties GetFileProperties(string path, bool isDirectory = false)
    {
        FileProperties fileProperties;
        FileInfo fileInfo;

        if (!isDirectory)
        {
            var fileAttributes = File.GetAttributes(path);
            fileInfo = new FileInfo(path);
            fileProperties = new FileProperties(path: path, size: fileInfo.Length, thumbnailPath: Option.None<string>());
            return fileProperties;
        }
        // move to config file and make sure the true extesion is specified (magic number)
        string[] videoExtensionPatterns = ["*.mp4"];

        var videoFile = videoExtensionPatterns.SelectMany(pattern => Directory.GetFiles(path, pattern))
            .FirstOrNone();

        if (!videoFile.HasValue)
        {
            throw new CannotIndexFileException($"The directory {path} does not contains a file that can be reproduced, so, it will not be indexed yet.");
        }

        fileInfo = new FileInfo(videoFile.ValueOrFailure());

        var thumbnail = Directory.GetFiles(path, "*.thumb.*").FirstOrNone();

        return new FileProperties(videoFile.ValueOrFailure(), fileInfo.Length, thumbnail);
    }

    public bool CreateSymbolicLink(string sourceFilePath, string destinationFilePath)
    {
        try
        {
            File.CreateSymbolicLink(destinationFilePath, sourceFilePath);
            _logger.LogInformation("Sybolic link {} successfuly created", destinationFilePath);
            return true;
        }
        catch(Exception ex)
        {
            _logger.LogError("Could not create symbolic link {}. Error: {}", destinationFilePath, ex);
            return false;
        }
    }
}
