namespace StreamingVideoIndexer.Infra.Settings;

public class S3Config
{
    public required string BucketName { get; set; }
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
    public required string Url { get; set; }
}
