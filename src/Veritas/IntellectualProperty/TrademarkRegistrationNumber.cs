using System;

namespace Veritas.IntellectualProperty;

/// <summary>Validation and generation for trademark registration numbers.</summary>
/// <example>
/// <code language="csharp">
/// Span&lt;char&gt; dst = stackalloc char[8];
/// TrademarkRegistrationNumber.TryGenerate(default, dst, out _);
/// </code>
/// </example>
public static class TrademarkRegistrationNumber
{
    private static readonly string[] _prefixes = { "TM", "TR", "MR" };

    /// <summary>Validates a trademark registration number.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<TrademarkRegistrationNumberValue> result)
    {
        Span<char> buf = stackalloc char[8];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (len >= 8) { result = new ValidationResult<TrademarkRegistrationNumberValue>(false, default, ValidationError.Length); return false; }
            char c = ch;
            if (c >= 'a' && c <= 'z') c = (char)(c - 32);
            if (len < 2)
            {
                if (c < 'A' || c > 'Z') { result = new ValidationResult<TrademarkRegistrationNumberValue>(false, default, ValidationError.Charset); return false; }
            }
            else
            {
                if (c < '0' || c > '9') { result = new ValidationResult<TrademarkRegistrationNumberValue>(false, default, ValidationError.Charset); return false; }
            }
            buf[len++] = c;
        }
        if (len != 8)
        {
            result = new ValidationResult<TrademarkRegistrationNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<TrademarkRegistrationNumberValue>(true, new TrademarkRegistrationNumberValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a trademark registration number into the destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a trademark registration number using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 8) { written = 0; return false; }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        var prefix = _prefixes[rng.Next(_prefixes.Length)];
        destination[0] = prefix[0];
        destination[1] = prefix[1];
        for (int i = 0; i < 6; i++) destination[2 + i] = (char)('0' + rng.Next(10));
        written = 8;
        return true;
    }
}
