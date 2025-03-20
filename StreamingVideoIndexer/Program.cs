using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using StreamingVideoIndexer.Core.Interfaces.Repositories;
using StreamingVideoIndexer.Core.Interfaces.Services;
using StreamingVideoIndexer.Core.Services;
using StreamingVideoIndexer.Core.Settings;
using StreamingVideoIndexer.Infra.DatabaseContext;
using StreamingVideoIndexer.Infra.Repositories;
using StreamingVideoIndexer.Infra.Services;
using StreamingVideoIndexer.Shared.Interfaces;
using StreamingVideoIndexer.Shared.Services;
using StreamingVideoIndexer.Infra.Settings;

namespace StreamingVideoIndexer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.Configure<IndexerProperties>(builder.Configuration.GetSection("IndexerProperties"));
        builder.Services.Configure<S3Config>(builder.Configuration.GetSection("S3Config"));

        var configuration = builder.Configuration;
        builder.Services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("StreamingVideoIndexer"));
        });

        builder.Services.AddHostedService<Worker>();
        
        // services
        builder.Services.AddSingleton<IHasherService, HasherService>();
        builder.Services.AddSingleton<IHandleFileService, HandleFileService>();
        builder.Services.AddSingleton<IDirectoryWatcher, DirectoryWatcher>();
        builder.Services.AddSingleton<IIndexFileService, IndexFileService>();
        builder.Services.AddSingleton<IS3StorageService, S3StorageService>();
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Config = configuration.GetSection("S3Config").Get<S3Config>();

            var config = new AmazonS3Config
            {
                ServiceURL = s3Config?.Url,
                ForcePathStyle = true
            };


            return new AmazonS3Client(s3Config?.AccessKey, s3Config?.SecretKey, config);
        });

        // repositories
        builder.Services.AddSingleton<IIndexFilesRepository, IndexFilesRepository>();

        var host = builder.Build();
        host.Run();
    }
}