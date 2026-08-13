using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using NatMyatSi.Models;
using NatMyatSi.Services;

namespace NatMyatSi.Pages;

public partial class Home : IAsyncDisposable
{
    private static readonly ProgressStep[] ProgressSteps =
    [
        new(1, "Question", "မေးခွန်း"),
        new(2, "Remember", "မှတ်သား"),
        new(3, "Choose", "ရွေးချယ်"),
        new(4, "Reading", "ဟောစာတမ်း")
    ];

    [Inject]
    private NatMyatSiDataService DataService { get; set; } = default!;

    [Inject]
    private ILogger<Home> Logger { get; set; } = default!;

    private readonly List<DivinationSymbol> shuffledSymbols = [];
    private CancellationTokenSource? loadCancellation;
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

    private int CurrentProgressStep => stage switch
    {
        WorkflowStage.Question => 1,
        WorkflowStage.Preview => 2,
        WorkflowStage.Selection => 3,
        WorkflowStage.Result => 4,
        _ => 0
    };

    protected override Task OnInitializedAsync() => LoadCatalogAsync();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
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

    private void StartReading() => MoveTo(
        WorkflowStage.Question,
        "Choose a question. / မေးခွန်းတစ်ခု ရွေးပါ။");

    private void SelectQuestion(NatMyatSiQuestion question)
    {
        selectedQuestion = question;
        PrepareShuffledSymbols();
        MoveTo(
            WorkflowStage.Preview,
            "The symbols are ready to remember. / သင်္ကေတများကို မှတ်သားရန် အဆင်သင့်ဖြစ်ပါပြီ။");
    }

    private void CoverSymbols() => MoveTo(
        WorkflowStage.Selection,
        "The symbols are covered. Choose one position. / သင်္ကေတများကို ဖုံးထားပါပြီ။ နေရာတစ်ခု ရွေးပါ။");

    private void ShowSymbolsAgain() => MoveTo(
        WorkflowStage.Preview,
        "The symbols are visible again. / သင်္ကေတများကို ပြန်လည်ပြသထားပါသည်။");

    private void RevealReading(DivinationSymbol symbol)
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
    }

    private void ChooseAnotherQuestion()
    {
        selectedQuestion = null;
        selectedSymbol = null;
        selectedReading = null;
        shuffledSymbols.Clear();
        MoveTo(
            WorkflowStage.Question,
            "Choose a new question. / မေးခွန်းအသစ်တစ်ခု ရွေးပါ။");
    }

    private void TrySameQuestion()
    {
        selectedSymbol = null;
        selectedReading = null;
        PrepareShuffledSymbols();
        MoveTo(
            WorkflowStage.Preview,
            "The symbols were shuffled again. / သင်္ကေတများကို ထပ်မံရောမွှေထားပါသည်။");
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
        if (loadCancellation is null)
        {
            return;
        }

        await loadCancellation.CancelAsync();
        loadCancellation.Dispose();
    }

    private sealed record ProgressStep(int Number, string English, string Myanmar);

    private enum WorkflowStage
    {
        Welcome,
        Question,
        Preview,
        Selection,
        Result
    }
}
