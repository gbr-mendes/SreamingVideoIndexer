﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Optional.Unsafe;
using StreamingVideoIndexer.Core.Interfaces.Repositories;
using StreamingVideoIndexer.Core.Interfaces.Services;
using StreamingVideoIndexer.Core.Models;
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
    private readonly IIndexFilesRepository _indexFilesRepository;
    private readonly IS3StorageService _s3StorageService;

    public IndexFileService(
        IHasherService hasherService,
        IOptions<IndexerProperties> indexerProperties,
        ILogger<IndexFileService> logger,
        IHandleFileService handleFileService,
        IIndexFilesRepository indexFilesRepository,
        IS3StorageService s3StorageService)
    {
        _hasherService = hasherService;
        _indexerProperties = indexerProperties.Value;
        _logger = logger;
        _handleFileService = handleFileService;
        _indexFilesRepository = indexFilesRepository;
        _s3StorageService = s3StorageService;
    }

    public async Task IndexFile(FileProperties fileProperties, bool reindex = false)
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

        if (!WasFileIndexed(symbolicLinkPath))
        {
            _logger.LogError("Error creating symbolic link for file {filePath}.", fileProperties.Path);
            return;
        }

        var indexedFile = new IndexedFile
        {
            Name = fileProperties.Name,
            Path = fileProperties.Path,
            Size = fileProperties.Size,
            ThumbnailPath = fileProperties.ThumbnailPath.HasValue ? fileProperties.ThumbnailPath.ValueOrFailure() : null,
            Duration = fileProperties.Duration,
            Description = fileProperties.Description
        };
        
        if (fileProperties.ThumbnailPath.HasValue)
        {
            var thumbnailPath = fileProperties.ThumbnailPath.ValueOrFailure();
            var thumHash = _hasherService.CalculateMd5(thumbnailPath);
            var thumbExtension = Path.GetExtension(thumbnailPath);
            var remoteKey = $"{thumHash}{thumbExtension}";
            
            await UploadThumbnail(thumbnailPath, remoteKey);

            indexedFile.ThumbnailRemoteKey = remoteKey;
        }

        await _indexFilesRepository.AddIndexedFileAsync(indexedFile);
        

        _logger.LogInformation("File {filePath} successfuly indexed.", fileProperties.Path);
        if (fileProperties.ThumbnailPath.HasValue)
        {
            _logger.LogInformation("Thumb path: {}", fileProperties.ThumbnailPath.ValueOrFailure());
        }

        // TODO: add try catch and rollbacK ater creating a symbolic lin in case of an erro adding on database
    }

    private async Task UploadThumbnail(string thumbnailPath, string remoteKey)
    {
        var wasThumbnailUploaded = await _s3StorageService.UploadFileAsync(remoteKey, thumbnailPath);
        if (!wasThumbnailUploaded)
        {
            _logger.LogError("Unexpected error uploading thumbnail {thumbnailPath} to S3.", thumbnailPath);
        }
        _logger.LogInformation("Thumbnail {thumbnailPath} uploaded to S3.", thumbnailPath);
    }

    private static bool WasFileIndexed(string filePath)
    {
        return File.Exists(filePath);
    }
}
