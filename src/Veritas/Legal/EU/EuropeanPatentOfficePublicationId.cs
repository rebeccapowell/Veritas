using System;

namespace Veritas.Legal.EU;

/// <summary>Validation and generation for European Patent Office publication identifiers.</summary>
/// <example>
/// <code language="csharp">
/// Span&lt;char&gt; dst = stackalloc char[11];
/// EuropeanPatentOfficePublicationId.TryGenerate(default, dst, out _);
/// </code>
/// </example>
public static class EuropeanPatentOfficePublicationId
{
    private static readonly string[] _kindCodes = { "A1", "B1" };

    /// <summary>Validates the supplied publication identifier.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EuropeanPatentOfficePublicationIdValue> result)
    {
        Span<char> buf = stackalloc char[11];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (len >= 11) { result = new ValidationResult<EuropeanPatentOfficePublicationIdValue>(false, default, ValidationError.Length); return false; }
            char c = ch;
            if (c >= 'a' && c <= 'z') c = (char)(c - 32);
            buf[len++] = c;
        }
        if (len != 11 || buf[0] != 'E' || buf[1] != 'P')
        {
            result = new ValidationResult<EuropeanPatentOfficePublicationIdValue>(false, default, ValidationError.Format);
            return false;
        }
        for (int i = 2; i < 9; i++) if (buf[i] < '0' || buf[i] > '9') { result = new ValidationResult<EuropeanPatentOfficePublicationIdValue>(false, default, ValidationError.Charset); return false; }
        if (buf[9] < 'A' || buf[9] > 'Z' || buf[10] < '0' || buf[10] > '9')
        {
            result = new ValidationResult<EuropeanPatentOfficePublicationIdValue>(false, default, ValidationError.Format);
            return false;
        }
        result = new ValidationResult<EuropeanPatentOfficePublicationIdValue>(true, new EuropeanPatentOfficePublicationIdValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Generates an EPO publication identifier into the destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates an EPO publication identifier using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 11) { written = 0; return false; }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = 'E';
        destination[1] = 'P';
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
