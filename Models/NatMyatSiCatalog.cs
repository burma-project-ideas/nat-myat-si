namespace NatMyatSi.Models;

public sealed class NatMyatSiCatalog
{
    private readonly IReadOnlyDictionary<(int QuestionId, string SymbolId), NatMyatSiReading> readings;

    public NatMyatSiCatalog(
        IReadOnlyList<NatMyatSiQuestion> questions,
        IReadOnlyList<DivinationSymbol> symbols,
        IReadOnlyList<NatMyatSiReading> answers)
    {
        ArgumentNullException.ThrowIfNull(questions);
        ArgumentNullException.ThrowIfNull(symbols);
        ArgumentNullException.ThrowIfNull(answers);

        if (questions.Count != 100)
        {
            throw new InvalidDataException("The NatMyatSi catalog must contain 100 questions.");
        }

        if (symbols.Count != 16)
        {
            throw new InvalidDataException("The NatMyatSi catalog must contain 16 symbols.");
        }

        var questionIds = questions.Select(question => question.Id).ToHashSet();
        var symbolIds = symbols.Select(symbol => symbol.Id).ToHashSet(StringComparer.Ordinal);

        if (questionIds.Count != questions.Count || symbolIds.Count != symbols.Count)
        {
            throw new InvalidDataException("The NatMyatSi catalog contains duplicate identifiers.");
        }

        var readingIndex = new Dictionary<(int QuestionId, string SymbolId), NatMyatSiReading>();
        foreach (var reading in answers)
        {
            if (!questionIds.Contains(reading.QuestionId) || !symbolIds.Contains(reading.SymbolId))
            {
                throw new InvalidDataException("A NatMyatSi reading points to an unknown question or symbol.");
            }

            if (!readingIndex.TryAdd((reading.QuestionId, reading.SymbolId), reading))
            {
                throw new InvalidDataException("The NatMyatSi catalog contains a duplicate reading.");
            }
        }

        foreach (var question in questions)
        {
            foreach (var symbol in symbols)
            {
                if (!readingIndex.ContainsKey((question.Id, symbol.Id)))
                {
                    throw new InvalidDataException("The NatMyatSi catalog is missing a reading.");
                }
            }
        }

        Questions = questions.OrderBy(question => question.Id).ToArray();
        Symbols = symbols.ToArray();
        readings = readingIndex;
    }

    public IReadOnlyList<NatMyatSiQuestion> Questions { get; }

    public IReadOnlyList<DivinationSymbol> Symbols { get; }

    public NatMyatSiReading GetReading(int questionId, string symbolId)
    {
        if (readings.TryGetValue((questionId, symbolId), out var reading))
        {
            return reading;
        }

        throw new KeyNotFoundException("The requested NatMyatSi reading does not exist.");
    }
}
