using StreamingVideoIndexer;
using StreamingVideoIndexer.Core.Interfaces.Services;
using StreamingVideoIndexer.Core.Services;
using StreamingVideoIndexer.Core.Settings;
using StreamingVideoIndexer.Shared.Interfaces;
using StreamingVideoIndexer.Shared.Services;

namespace SreamingVideoIndexer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.Configure<IndexerProperties>(builder.Configuration.GetSection("IndexerProperties"));

        builder.Services.AddHostedService<Worker>();
        builder.Services.AddSingleton<IHasherService, HasherService>();
        builder.Services.AddSingleton<IHandleFileService, HandleFileService>();
        builder.Services.AddSingleton<IDirectoryWatcher, DirectoryWatcher>();
        builder.Services.AddSingleton<IIndexFileService, IndexFileService>();

        var host = builder.Build();
        host.Run();
    }
}