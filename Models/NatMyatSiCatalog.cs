namespace NatMyatSi.Models;

public sealed class NatMyatSiCatalog
{
    private readonly IReadOnlyDictionary<(int QuestionId, string SymbolId), NatMyatSiReading> readings;

    public NatMyatSiCatalog(NatMyatSiData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.Questions.Count != 100)
        {
            throw new InvalidDataException("The NatMyatSi catalog must contain 100 questions.");
        }

        if (data.Symbols.Count != 16)
        {
            throw new InvalidDataException("The NatMyatSi catalog must contain 16 symbols.");
        }

        var questionIds = data.Questions.Select(question => question.Id).ToHashSet();
        var symbolIds = data.Symbols.Select(symbol => symbol.Id).ToHashSet(StringComparer.Ordinal);

        if (questionIds.Count != data.Questions.Count || symbolIds.Count != data.Symbols.Count)
        {
            throw new InvalidDataException("The NatMyatSi catalog contains duplicate identifiers.");
        }

        var readingIndex = new Dictionary<(int QuestionId, string SymbolId), NatMyatSiReading>();
        foreach (var reading in data.Answers)
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

        foreach (var question in data.Questions)
        {
            foreach (var symbol in data.Symbols)
            {
                if (!readingIndex.ContainsKey((question.Id, symbol.Id)))
                {
                    throw new InvalidDataException("The NatMyatSi catalog is missing a reading.");
                }
            }
        }

        Questions = data.Questions.OrderBy(question => question.Id).ToArray();
        Symbols = data.Symbols.ToArray();
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
