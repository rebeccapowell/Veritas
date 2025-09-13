using System;
using Veritas;

namespace Veritas.Tax.BG;

public readonly struct EgnValue
{
    public string Value { get; }
    public EgnValue(string value) => Value = value;
}

/// <summary>
/// Bulgaria EGN personal number (10 digits, weighted checksum).
/// </summary>
public static class Egn
{
    private static readonly int[] Weights = { 2, 4, 8, 5, 10, 9, 7, 3, 6 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EgnValue> result)
    {
        Span<char> digits = stackalloc char[10];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<EgnValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 10)
        {
            result = new ValidationResult<EgnValue>(false, default, ValidationError.Length);
            return false;
        }
        int check = ComputeCheckDigit(digits[..9]);
        if (digits[9] - '0' != check)
        {
            result = new ValidationResult<EgnValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<EgnValue>(true, new EgnValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 10) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..10];
        int year = rng.Next(1900, 2100);
        int month = rng.Next(1, 13);
        int day = rng.Next(1, DateTime.DaysInMonth(year, month) + 1);
        int monthField = month;
        if (year < 1900) monthField += 20;
        else if (year >= 2000) monthField += 40;
        digits[0] = (char)('0' + (year % 100 / 10));
        digits[1] = (char)('0' + (year % 10));
        digits[2] = (char)('0' + (monthField / 10));
        digits[3] = (char)('0' + (monthField % 10));
        digits[4] = (char)('0' + (day / 10));
        digits[5] = (char)('0' + (day % 10));
        for (int i = 6; i < 9; i++)
            digits[i] = (char)('0' + rng.Next(10));
        digits[9] = (char)('0' + ComputeCheckDigit(digits[..9]));
        written = 10;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        for (int i = 0; i < Weights.Length; i++)
            sum += (digits[i] - '0') * Weights[i];
        int r = sum % 11;
        return r == 10 ? 0 : r;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}

