using System.Net.Http.Json;
using NatMyatSi.Models;

namespace NatMyatSi.Services;

public sealed class NatMyatSiDataService(HttpClient httpClient)
{
    private const string CatalogPath = "data/nat-myat-si.json";
    private readonly SemaphoreSlim loadLock = new(1, 1);
    private NatMyatSiCatalog? catalog;

    public async Task<NatMyatSiCatalog> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        if (catalog is not null)
        {
            return catalog;
        }

        await loadLock.WaitAsync(cancellationToken);
        try
        {
            if (catalog is not null)
            {
                return catalog;
            }

            var data = await httpClient.GetFromJsonAsync(
                CatalogPath,
                NatMyatSiJsonContext.Default.NatMyatSiData,
                cancellationToken);

            catalog = data is null
                ? throw new InvalidDataException("The NatMyatSi catalog could not be read.")
                : new NatMyatSiCatalog(data);

            return catalog;
        }
        finally
        {
            loadLock.Release();
        }
    }
}
