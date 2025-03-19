using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StreamingVideoIndexer.Core.Interfaces.Services;
using StreamingVideoIndexer.Core.Settings;
using StreamingVideoIndexer.Core.ValueObjects;
using System.Collections.Concurrent;

namespace StreamingVideoIndexer.Core.Services;

public class DirectoryWatcher : IDirectoryWatcher
{
    private readonly IndexerProperties _indexerProperties;
    private readonly ILogger<DirectoryWatcher> _logger;
    private readonly IHandleFileService _handleFileService;
    private ConcurrentQueue<FileProperties>? _filesToIndex;

    public DirectoryWatcher(
        IOptions<IndexerProperties> indexerProperties,
        ILogger<DirectoryWatcher> logger,
        IHandleFileService handleFileService)
    {
        _indexerProperties = indexerProperties.Value;
        _logger = logger;
        _handleFileService = handleFileService;
        ConfigureWatcher();
    }

    public void OnCreated(object sender, FileSystemEventArgs e)
    {
        var attributes = File.GetAttributes(e.FullPath);
        var isDirectory = attributes.HasFlag(FileAttributes.Directory);

        try
        {
            var fileProperties = _handleFileService.GetFileProperties(e.FullPath, isDirectory);
            _filesToIndex?.Enqueue(fileProperties);
        }
        catch(Exception ex)
        {
            _logger.LogError("Could not index {filePath}. Error: {ex}", e.FullPath, ex.Message);
        }
    }

    public void Start(ConcurrentQueue<FileProperties> filesToIndex)
    {
        _filesToIndex = filesToIndex;
        _logger.LogInformation("DirectoryWatcher start monitoring directory {}", _indexerProperties.SearchDirectory);
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    private void ConfigureWatcher()
    {
        var watcher = new FileSystemWatcher(_indexerProperties.SearchDirectory)
        {
            NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size
        };

        watcher.Created += OnCreated;

        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
    }

    
}
