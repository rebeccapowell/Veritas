using System;
using Veritas;

namespace Veritas.Identity.Israel;

public readonly struct TeudatZehutValue
{
    public string Value { get; }
    public TeudatZehutValue(string value) => Value = value;
}

/// <summary>
/// Israel Teudat Zehut (9 digits, weighted mod-10 checksum).
/// </summary>
public static class TeudatZehut
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<TeudatZehutValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<TeudatZehutValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 9)
        {
            result = new ValidationResult<TeudatZehutValue>(false, default, ValidationError.Length);
            return true;
        }
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int d = digits[i] - '0';
            if ((uint)d > 9) { result = new ValidationResult<TeudatZehutValue>(false, default, ValidationError.Charset); return true; }
            int v = d * ((i % 2) + 1);
            if (v > 9) v -= 9;
            sum += v;
        }
        if (sum % 10 != 0)
        {
            result = new ValidationResult<TeudatZehutValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<TeudatZehutValue>(true, new TeudatZehutValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 9) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..9];
        for (int i = 0; i < 8; i++)
            digits[i] = (char)('0' + rng.Next(10));
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            int d = digits[i] - '0';
            int v = d * ((i % 2) + 1);
            if (v > 9) v -= 9;
            sum += v;
        }
        digits[8] = (char)('0' + ((10 - (sum % 10)) % 10));
        written = 9;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (!char.IsDigit(ch) || len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}
