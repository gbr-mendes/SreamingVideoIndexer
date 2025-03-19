using Microsoft.Extensions.Options;
using StreamingVideoIndexer.Core.Exceptions;
using StreamingVideoIndexer.Core.Interfaces.Services;
using StreamingVideoIndexer.Core.Settings;
using StreamingVideoIndexer.Core.ValueObjects;
using System.Collections.Concurrent;

namespace StreamingVideoIndexer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IndexerProperties _indexerProperties;
    private readonly IHandleFileService _handleFileService;
    private readonly IDirectoryWatcher _directoryWatcher;
    private readonly ConcurrentQueue<FileProperties> _filesToIndex;
    private readonly IIndexFileService _indexFileService;

    public Worker(
        ILogger<Worker> logger,
        IOptions<IndexerProperties> indexerProperties,
        IHandleFileService handleFileService,
        IDirectoryWatcher directoryWatcher,
        IIndexFileService indexFileService)
    {
        _logger = logger;
        _indexerProperties = indexerProperties.Value;
        _handleFileService = handleFileService;
        _directoryWatcher = directoryWatcher;
        _indexFileService = indexFileService;
        _filesToIndex = new ConcurrentQueue<FileProperties>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ProcessEntriesOnStart();
        _directoryWatcher.Start(_filesToIndex);
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                while (_filesToIndex.TryDequeue(out var file))
                {
                    await _indexFileService.IndexFile(file);
                }
            }
            await Task.Delay(1000, stoppingToken);
        }
    }

    protected async void ProcessEntriesOnStart()
    {
        var entries = Directory.GetFileSystemEntries(_indexerProperties.SearchDirectory);
        foreach (var path in entries)
        {
            if (path == _indexerProperties.IndexedVideosDirectory)
            {
                continue;
            }
            var attributes = File.GetAttributes(path);
            var isDirectory = attributes.HasFlag(FileAttributes.Directory);

            try
            {
                var fileProperties = await _handleFileService.GetFileProperties(path, isDirectory);
                await _indexFileService.IndexFile(fileProperties);
            }
            catch(CannotIndexFileException ex)
            {
                _logger.LogError("Error while indexing path {path}: {error}", path, ex.Message);
                continue;
            }
        }
    }
}
