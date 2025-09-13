using System;
using Veritas;

namespace Veritas.Tax.GR;

public readonly struct AfmValue
{
    public string Value { get; }
    public AfmValue(string value) => Value = value;
}

/// <summary>
/// Greek AFM tax identifier (9 digits, mod-11 checksum with powers-of-two weighting).
/// </summary>
public static class Afm
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<AfmValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<AfmValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9)
        {
            result = new ValidationResult<AfmValue>(false, default, ValidationError.Length);
            return false;
        }
        int sum = 0;
        for (int i = 0; i < 8; i++)
            sum += (digits[i] - '0') * (1 << (8 - i));
        int expected = sum % 11 % 10;
        if (digits[8] - '0' != expected)
        {
            result = new ValidationResult<AfmValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<AfmValue>(true, new AfmValue(new string(digits)), ValidationError.None);
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
            sum += (digits[i] - '0') * (1 << (8 - i));
        int check = sum % 11 % 10;
        digits[8] = (char)('0' + check);
        written = 9;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (!char.IsDigit(ch) || len >= dest.Length)
            {
                len = 0;
                return false;
            }
            dest[len++] = ch;
        }
        return true;
    }
}

