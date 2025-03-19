using Microsoft.Extensions.DependencyInjection;
using StreamingVideoIndexer.Core.Interfaces.Repositories;
using StreamingVideoIndexer.Core.Models;
using DataContext = StreamingVideoIndexer.Infra.DatabaseContext.DatabaseContext;

namespace StreamingVideoIndexer.Infra.Repositories;

public class IndexFilesRepository : IIndexFilesRepository
{
    private readonly IServiceScopeFactory _scopeFacotry;

    public IndexFilesRepository(IServiceScopeFactory scopeFactory)
    {
        _scopeFacotry = scopeFactory;
    }

    public async Task AddIndexedFileAsync(IndexedFile indexedFile)
    {
        using var scope = _scopeFacotry.CreateScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await databaseContext.IndexedFiles.AddAsync(indexedFile);
        await databaseContext.SaveChangesAsync();
    }
}
