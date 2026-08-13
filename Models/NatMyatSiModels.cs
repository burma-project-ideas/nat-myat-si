namespace NatMyatSi.Models;

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
    public int QuestionId { get; init; }

    public string SymbolId { get; init; } = string.Empty;

    public string Text { get; init; } = string.Empty;
}
