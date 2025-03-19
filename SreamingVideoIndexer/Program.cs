using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StreamingVideoIndexer;
using StreamingVideoIndexer.Core.Interfaces.Services;
using StreamingVideoIndexer.Core.Services;
using StreamingVideoIndexer.Core.Settings;
using StreamingVideoIndexer.Infra.DatabaseContext;
using StreamingVideoIndexer.Shared.Interfaces;
using StreamingVideoIndexer.Shared.Services;

namespace SreamingVideoIndexer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.Configure<IndexerProperties>(builder.Configuration.GetSection("IndexerProperties"));
        var configuration = builder.Configuration;
        builder.Services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("StreamingVideoIndexer"));
        });

        builder.Services.AddHostedService<Worker>();
        builder.Services.AddSingleton<IHasherService, HasherService>();
        builder.Services.AddSingleton<IHandleFileService, HandleFileService>();
        builder.Services.AddSingleton<IDirectoryWatcher, DirectoryWatcher>();
        builder.Services.AddSingleton<IIndexFileService, IndexFileService>();

        var host = builder.Build();
        host.Run();
    }
}