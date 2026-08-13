using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NatMyatSi.Models;
using NatMyatSi.Services;

namespace NatMyatSi.Pages;

public partial class Home : IAsyncDisposable
{
    private static readonly ProgressStep[] ProgressSteps =
    [
        new(1, "Question", "မေးခွန်း"),
        new(2, "Position", "နေရာ"),
        new(3, "Reading", "ဟောစာတမ်း")
    ];

    [Inject]
    private NatMyatSiDataService DataService { get; set; } = default!;

    [Inject]
    private ILogger<Home> Logger { get; set; } = default!;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = default!;

    private readonly List<DivinationSymbol> shuffledSymbols = [];
    private CancellationTokenSource? loadCancellation;
    private InterfaceSoundInterop? soundInterop;
    private ElementReference mainContent;
    private NatMyatSiCatalog? catalog;
    private NatMyatSiQuestion? selectedQuestion;
    private DivinationSymbol? selectedSymbol;
    private NatMyatSiReading? selectedReading;
    private WorkflowStage stage = WorkflowStage.Welcome;
    private string? loadError;
    private string statusAnnouncement = string.Empty;
    private bool isLoading = true;
    private bool focusMainAfterRender;
    private bool soundEnabled = true;

    private int CurrentProgressStep => stage switch
    {
        WorkflowStage.Question => 1,
        WorkflowStage.Selection => 2,
        WorkflowStage.Result => 3,
        _ => 0
    };

    protected override Task OnInitializedAsync() => LoadCatalogAsync();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            soundInterop = new InterfaceSoundInterop(JsRuntime);
        }

        if (!focusMainAfterRender)
        {
            return;
        }

        focusMainAfterRender = false;
        await mainContent.FocusAsync();
    }

    private async Task LoadCatalogAsync()
    {
        if (loadCancellation is not null)
        {
            await loadCancellation.CancelAsync();
            loadCancellation.Dispose();
        }

        loadCancellation = new CancellationTokenSource();
        var cancellationToken = loadCancellation.Token;

        isLoading = true;
        loadError = null;

        try
        {
            catalog = await DataService.GetCatalogAsync(cancellationToken);
            statusAnnouncement = "Questions are ready. / မေးခွန်းများ အဆင်သင့်ဖြစ်ပါပြီ။";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Failed to load the local NatMyatSi catalog.");
            loadError = "Unable to load local reading data.";
        }
        finally
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                isLoading = false;
            }
        }
    }

    private async Task StartReading()
    {
        MoveTo(
            WorkflowStage.Question,
            "Choose a question. / မေးခွန်းတစ်ခု ရွေးပါ။");
        await PlaySoundAsync("navigate");
    }

    private async Task SelectQuestion(NatMyatSiQuestion question)
    {
        selectedQuestion = question;
        PrepareShuffledSymbols();
        MoveTo(
            WorkflowStage.Selection,
            "Choose a covered position. / ဖုံးထားသော နေရာတစ်ခု ရွေးပါ။");
        await PlaySoundAsync("target");
    }

    private async Task RevealReading(DivinationSymbol symbol)
    {
        if (catalog is null || selectedQuestion is null)
        {
            return;
        }

        selectedSymbol = symbol;
        selectedReading = catalog.GetReading(selectedQuestion.Id, symbol.Id);
        MoveTo(
            WorkflowStage.Result,
            $"Your symbol is {symbol.Name}. The reading is ready. / သင်၏သင်္ကေတမှာ {symbol.MyanmarName} ဖြစ်သည်။ ဟောစာတမ်း အဆင်သင့်ဖြစ်ပါပြီ။");
        await PlaySoundAsync("complete");
    }

    private async Task ChooseAnotherQuestion()
    {
        selectedQuestion = null;
        selectedSymbol = null;
        selectedReading = null;
        shuffledSymbols.Clear();
        MoveTo(
            WorkflowStage.Question,
            "Choose another question. / အခြားမေးခွန်းတစ်ခု ရွေးပါ။");
        await PlaySoundAsync("navigate");
    }

    private async Task TrySameQuestion()
    {
        selectedSymbol = null;
        selectedReading = null;
        PrepareShuffledSymbols();
        MoveTo(
            WorkflowStage.Selection,
            "The hidden symbols were shuffled again. Choose one position. / ဖုံးထားသော သင်္ကေတများကို ထပ်မံရောမွှေထားပါသည်။ နေရာတစ်ခု ရွေးပါ။");
        await PlaySoundAsync("target");
    }

    private async Task ToggleSoundAsync()
    {
        soundEnabled = !soundEnabled;
        await PlaySoundAsync(soundEnabled ? "activate" : "deactivate", force: true);
    }

    private async Task PlaySoundAsync(string cue, bool force = false)
    {
        if (!soundEnabled && !force)
        {
            return;
        }

        try
        {
            if (soundInterop is not null)
            {
                await soundInterop.PlayAsync(cue);
            }
        }
        catch (JSException exception)
        {
            Logger.LogWarning(exception, "The optional interface sound cue could not be played.");
        }
    }

    private void PrepareShuffledSymbols()
    {
        shuffledSymbols.Clear();
        shuffledSymbols.AddRange(catalog!.Symbols);
        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(shuffledSymbols));
    }

    private void MoveTo(WorkflowStage nextStage, string announcement)
    {
        stage = nextStage;
        statusAnnouncement = announcement;
        focusMainAfterRender = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (loadCancellation is not null)
        {
            await loadCancellation.CancelAsync();
            loadCancellation.Dispose();
        }

        if (soundInterop is not null)
        {
            await soundInterop.DisposeAsync();
        }
    }

    private sealed record ProgressStep(int Number, string English, string Myanmar);

    private enum WorkflowStage
    {
        Welcome,
        Question,
        Selection,
        Result
    }
}
