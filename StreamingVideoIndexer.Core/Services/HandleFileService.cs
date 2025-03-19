using Microsoft.Extensions.Logging;
using Optional.Collections;
using Optional;
using StreamingVideoIndexer.Core.Interfaces.Services;
using StreamingVideoIndexer.Core.ValueObjects;
using Optional.Unsafe;
using StreamingVideoIndexer.Core.Exceptions;
using MediaInfo.DotNetWrapper.Enumerations;

namespace StreamingVideoIndexer.Core.Services;

public class HandleFileService : IHandleFileService
{
    private readonly ILogger<HandleFileService> _logger;
    public HandleFileService(ILogger<HandleFileService> logger)
    {
        _logger = logger;
    }

    public async Task<FileProperties> GetFileProperties(string path, bool isDirectory = false)
    {
        FileProperties fileProperties;
        FileInfo fileInfo;
        TimeSpan duration;
        string fileName;

        if (!isDirectory)
        {
            duration = GetVideoDuration(path);
            var fileAttributes = File.GetAttributes(path);
            fileInfo = new FileInfo(path);
            fileName = Path.GetFileNameWithoutExtension(path);
            fileProperties = new FileProperties(name: fileName, path: path, size: fileInfo.Length, thumbnailPath: Option.None<string>(), duration: duration);
            return fileProperties;
        }
        // TODO: move to config file and make sure the true extesion is specified (magic number)
        string[] videoExtensionPatterns = ["*.mp4"];

        var videoFile = videoExtensionPatterns.SelectMany(pattern => Directory.GetFiles(path, pattern))
            .FirstOrNone();


        if (!videoFile.HasValue)
        {
            throw new CannotIndexFileException($"The directory {path} does not contains a file that can be reproduced, so, it will not be indexed yet.");
        }
        
        var descriptionFile = Directory.GetFiles(path, "*.desc.*").FirstOrNone();

        var description = descriptionFile.HasValue ? await ReadTextFileAsync(descriptionFile.ValueOrFailure()) : null;

        fileInfo = new FileInfo(videoFile.ValueOrFailure());

        var thumbnail = Directory.GetFiles(path, "*.thumb.*").FirstOrNone();
        duration = GetVideoDuration(videoFile.ValueOrFailure());
       
        fileName = Path.GetFileNameWithoutExtension(path);

        return new FileProperties(fileName, videoFile.ValueOrFailure(), fileInfo.Length, thumbnail, duration, description);
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

    public static TimeSpan GetVideoDuration(string path)
    {
        var mediaInfo = new MediaInfo.DotNetWrapper.MediaInfo();
        mediaInfo.Open(path);
        var durationMilisseconds = long.Parse(mediaInfo.Get(StreamKind.Video, 0, "Duration"));
        return TimeSpan.FromMilliseconds(durationMilisseconds);
    }

    public static async Task<string> ReadTextFileAsync(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File {path} not found.");
        }

        using var reader = new StreamReader(path);
        return await reader.ReadToEndAsync();
    }
}
