using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StreamingVideoIndexer.Core.Interfaces.Services;
using StreamingVideoIndexer.Infra.Settings;

namespace StreamingVideoIndexer.Infra.Services;

public class S3StorageService : IS3StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<S3StorageService> _logger;
    private readonly S3Config _s3Config;

    public S3StorageService(IAmazonS3 s3Client, ILogger<S3StorageService> logger, IOptions<S3Config> s3Config)
    {
        _s3Client = s3Client;
        _logger = logger;
        _s3Config = s3Config.Value;
    }

    public async Task<bool> UploadFileAsync(string key, string filePath)
    {
        // TODO: For large files, its better to upload it on batches, so we do not load the entire file in memory
        var bucketName = _s3Config.BucketName;
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = new FileStream(filePath, FileMode.Open)
        };
        
        var resp = await _s3Client.PutObjectAsync(request);
        if (resp.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            _logger.LogError("Error uploading file {fileName} to {bucketName}. StatusCode: {statusCode}", filePath, bucketName, resp.HttpStatusCode);
            return false;
        }
       
       _logger.LogInformation("File {fileName} uploaded to {bucketName}. StatusCode: {statusCode}", filePath, bucketName, resp.HttpStatusCode);
        return true;
    }
}
