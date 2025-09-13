using System;

namespace Veritas.IntellectualProperty;

/// <summary>Validation and generation for WIPO ST.16 patent publication numbers.</summary>
/// <example>
/// <code language="csharp">
/// Span&lt;char&gt; dst = stackalloc char[11];
/// PatentPublicationNumber.TryGenerate(default, dst, out _);
/// </code>
/// </example>
public static class PatentPublicationNumber
{
    private static readonly string[] _countryCodes = { "US", "EP", "WO", "JP", "DE" };
    private static readonly string[] _kindCodes = { "A1", "B1", "A2" };

    /// <summary>Validates the supplied patent publication number.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PatentPublicationNumberValue> result)
    {
        Span<char> buf = stackalloc char[12];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (len >= 12) { result = new ValidationResult<PatentPublicationNumberValue>(false, default, ValidationError.Length); return false; }
            char c = ch;
            if (c >= 'a' && c <= 'z') c = (char)(c - 32);
            if (len < 2)
            {
                if (c < 'A' || c > 'Z') { result = new ValidationResult<PatentPublicationNumberValue>(false, default, ValidationError.Charset); return false; }
            }
            buf[len++] = c;
        }
        if (len < 5)
        {
            result = new ValidationResult<PatentPublicationNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        if (buf[len - 2] < 'A' || buf[len - 2] > 'Z' || buf[len - 1] < '0' || buf[len - 1] > '9')
        {
            result = new ValidationResult<PatentPublicationNumberValue>(false, default, ValidationError.Format);
            return false;
        }
        for (int i = 2; i < len - 2; i++)
        {
            if (buf[i] < '0' || buf[i] > '9')
            {
                result = new ValidationResult<PatentPublicationNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        result = new ValidationResult<PatentPublicationNumberValue>(true, new PatentPublicationNumberValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Generates a patent publication number into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a patent publication number using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 11) { written = 0; return false; }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        var cc = _countryCodes[rng.Next(_countryCodes.Length)];
        destination[0] = cc[0];
        destination[1] = cc[1];
        int serial = rng.Next(0, 10_000_000);
        for (int i = 0; i < 7; i++)
        {
            destination[8 - i] = (char)('0' + (serial % 10));
            serial /= 10;
        }
        var kind = _kindCodes[rng.Next(_kindCodes.Length)];
        destination[9] = kind[0];
        destination[10] = kind[1];
        written = 11;
        return true;
    }
}
