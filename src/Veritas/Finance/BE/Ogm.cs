using System;
using Veritas;

namespace Veritas.Finance.BE;

/// <summary>Belgian structured communication (OGM) reference.</summary>
public static class Ogm
{
    /// <summary>Attempts to validate a structured communication reference.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<OgmValue> result)
    {
        Span<char> digits = stackalloc char[12];
        if (!Normalize(input, digits, out int len) || len != 12)
        {
            result = new ValidationResult<OgmValue>(false, default, ValidationError.Length);
            return false;
        }
        long baseNumber = ParseLong(digits[..10]);
        int expected = (int)(baseNumber % 97);
        if (expected == 0) expected = 97;
        int check = (digits[10] - '0') * 10 + (digits[11] - '0');
        if (check != expected)
        {
            result = new ValidationResult<OgmValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<OgmValue>(true, new OgmValue(new string(digits)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a structured communication reference.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a structured communication reference.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 12) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..12];
        for (int i = 0; i < 10; i++)
            digits[i] = (char)('0' + rng.Next(10));
        long baseNumber = ParseLong(digits[..10]);
        int check = (int)(baseNumber % 97);
        if (check == 0) check = 97;
        digits[10] = (char)('0' + check / 10);
        digits[11] = (char)('0' + check % 10);
        written = 12;
        return true;
    }

    private static long ParseLong(ReadOnlySpan<char> digits)
    {
        long n = 0;
        foreach (var ch in digits)
            n = n * 10 + (ch - '0');
        return n;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '/' || ch == '-' || ch == '+')
                continue;
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
