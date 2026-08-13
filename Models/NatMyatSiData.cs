using System.Text.Json.Serialization;

namespace NatMyatSi.Models;

public sealed class NatMyatSiData
{
    public List<NatMyatSiQuestion> Questions { get; init; } = [];

    public List<DivinationSymbol> Symbols { get; init; } = [];

    public List<NatMyatSiReading> Answers { get; init; } = [];
}

public sealed class NatMyatSiQuestion
{
    public int Id { get; init; }

    public string Text { get; init; } = string.Empty;
}

public sealed class DivinationSymbol
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Symbol { get; init; } = string.Empty;

    public int Row { get; init; }

    public int Column { get; init; }

    [JsonIgnore]
    public string MyanmarName => Id switch
    {
        "jupiter" => "ကြာသပတေး",
        "saturn" => "စနေ",
        "sun" => "နေ",
        "mercury" => "ဗုဒ္ဓဟူး",
        "venus" => "သောကြာ",
        "moon" => "လ",
        "neptune" => "နက်ပကျွန်း",
        "pisces" => "မိန်",
        "aries" => "မိဿ",
        "capricorn" => "မကရ",
        "gemini" => "မေထုန်",
        "libra" => "တူ",
        "mars" => "အင်္ဂါ",
        "leo" => "သိဟ်",
        "virgo" => "ကန်",
        "taurus" => "ပြိဿ",
        _ => Name
    };
}

public sealed class NatMyatSiReading
{
    [JsonPropertyName("question_id")]
    public int QuestionId { get; init; }

    [JsonPropertyName("symbol_id")]
    public string SymbolId { get; init; } = string.Empty;

    public string Text { get; init; } = string.Empty;
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(NatMyatSiData))]
internal sealed partial class NatMyatSiJsonContext : JsonSerializerContext;
