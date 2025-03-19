using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Optional.Unsafe;
using StreamingVideoIndexer.Core.Interfaces.Services;
using StreamingVideoIndexer.Core.Settings;
using StreamingVideoIndexer.Core.ValueObjects;
using StreamingVideoIndexer.Shared.Interfaces;

namespace StreamingVideoIndexer.Core.Services;

public class IndexFileService : IIndexFileService
{
    private readonly IHasherService _hasherService;
    private readonly IndexerProperties _indexerProperties;
    private readonly ILogger<IndexFileService> _logger;
    private readonly IHandleFileService _handleFileService;

    public IndexFileService(
        IHasherService hasherService,
        IOptions<IndexerProperties> indexerProperties,
        ILogger<IndexFileService> logger,
        IHandleFileService handleFileService)
    {
        _hasherService = hasherService;
        _indexerProperties = indexerProperties.Value;
        _logger = logger;
        _handleFileService = handleFileService;
    }

    public void IndexFile(FileProperties fileProperties, bool reindex = false)
    {
        
        var fileHash = _hasherService.CalculateMd5(fileProperties.Path);
        var symbolicLinkPath = Path.Combine(_indexerProperties.IndexedVideosDirectory, fileHash);
        var extension = Path.GetExtension(fileProperties.Path);
        symbolicLinkPath = Path.ChangeExtension(symbolicLinkPath, extension);

        if (WasFileIndexed(symbolicLinkPath) && !reindex)
        {
            _logger.LogInformation("File {filePath} already indexed. Skipping.", fileProperties.Path);
            return;
        }

        _handleFileService.CreateSymbolicLink(fileProperties.Path, symbolicLinkPath);
        // add properties on db
        _logger.LogInformation("File {filePath} successfuly indexed.", fileProperties.Path);
        if (fileProperties.ThumbnailPath.HasValue)
        {
            _logger.LogInformation("Thumb path: {}", fileProperties.ThumbnailPath.ValueOrFailure());
        }
    }

    private static bool WasFileIndexed(string filePath)
    {
        return File.Exists(filePath);
    }
}
