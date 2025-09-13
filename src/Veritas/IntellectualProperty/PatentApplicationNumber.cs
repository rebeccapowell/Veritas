using System;

namespace Veritas.IntellectualProperty;

/// <summary>Validation and generation for WIPO ST.13 patent application numbers.</summary>
/// <example>
/// <code language="csharp">
/// Span&lt;char&gt; dst = stackalloc char[10];
/// PatentApplicationNumber.TryGenerate(default, dst, out _);
/// </code>
/// </example>
public static class PatentApplicationNumber
{
    private static readonly string[] _countryCodes = { "US", "GB", "JP", "DE", "FR" };

    /// <summary>Validates the supplied patent application number.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PatentApplicationNumberValue> result)
    {
        Span<char> buf = stackalloc char[10];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (len >= 10) { result = new ValidationResult<PatentApplicationNumberValue>(false, default, ValidationError.Length); return false; }
            char c = ch;
            if (len < 2)
            {
                if (c >= 'a' && c <= 'z') c = (char)(c - 32);
                if (c < 'A' || c > 'Z') { result = new ValidationResult<PatentApplicationNumberValue>(false, default, ValidationError.Charset); return false; }
            }
            else
            {
                if (c < '0' || c > '9') { result = new ValidationResult<PatentApplicationNumberValue>(false, default, ValidationError.Charset); return false; }
            }
            buf[len++] = c;
        }
        if (len < 5 || len > 10)
        {
            result = new ValidationResult<PatentApplicationNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        if (len > 4 && len - 4 > 6) { result = new ValidationResult<PatentApplicationNumberValue>(false, default, ValidationError.Length); return false; }
        result = new ValidationResult<PatentApplicationNumberValue>(true, new PatentApplicationNumberValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Generates a patent application number into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a patent application number using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 10) { written = 0; return false; }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        var cc = _countryCodes[rng.Next(_countryCodes.Length)];
        destination[0] = cc[0];
        destination[1] = cc[1];
        int year = rng.Next(0, 100);
        destination[2] = (char)('0' + year / 10);
        destination[3] = (char)('0' + year % 10);
        int serial = rng.Next(0, 1_000_000);
        for (int i = 0; i < 6; i++)
        {
            destination[9 - i] = (char)('0' + (serial % 10));
            serial /= 10;
        }
        written = 10;
        return true;
    }
}
