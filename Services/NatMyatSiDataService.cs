using NatMyatSi.Data;
using NatMyatSi.Models;

namespace NatMyatSi.Services;

public sealed class NatMyatSiDataService
{
    private readonly Lazy<NatMyatSiCatalog> catalog = new(
        NatMyatSiStaticData.CreateCatalog,
        LazyThreadSafetyMode.ExecutionAndPublication);

    public Task<NatMyatSiCatalog> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(catalog.Value);
    }
}
