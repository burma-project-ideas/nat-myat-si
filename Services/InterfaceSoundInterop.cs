using Microsoft.JSInterop;

namespace NatMyatSi.Services;

public sealed class InterfaceSoundInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    internal const string ModulePath = "./Pages/Home.razor.js";
    internal const string ImportMethod = "import";
    internal const string PlayMethod = "play";
    private IJSObjectReference? module;

    public async ValueTask PlayAsync(string cue)
    {
        module ??= await jsRuntime.InvokeAsync<IJSObjectReference>(ImportMethod, ModulePath);
        await module.InvokeVoidAsync(PlayMethod, cue);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
        }
    }
}
