using System;
using Veritas;

namespace Veritas.Tax.RO;

public readonly struct CnpValue
{
    public string Value { get; }
    public CnpValue(string value) => Value = value;
}

/// <summary>
/// Romania CNP personal numeric code (13 digits, mod-11 checksum).
/// </summary>
public static class Cnp
{
    private static readonly int[] Weights = { 2, 7, 9, 1, 4, 6, 3, 5, 8, 2, 7, 9 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CnpValue> result)
    {
        Span<char> digits = stackalloc char[13];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<CnpValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 13)
        {
            result = new ValidationResult<CnpValue>(false, default, ValidationError.Length);
            return true;
        }
        int check = ComputeCheckDigit(digits[..12]);
        if (digits[12] - '0' != check)
        {
            result = new ValidationResult<CnpValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<CnpValue>(true, new CnpValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 13) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..13];
        int year = rng.Next(1900, 2100);
        int month = rng.Next(1, 13);
        int day = rng.Next(1, DateTime.DaysInMonth(year, month) + 1);
        int s = year >= 2000 ? (rng.Next(2) == 0 ? 5 : 6) : 1 + rng.Next(2);
        digits[0] = (char)('0' + s);
        digits[1] = (char)('0' + (year / 10 % 10));
        digits[2] = (char)('0' + (year % 10));
        digits[3] = (char)('0' + (month / 10));
        digits[4] = (char)('0' + (month % 10));
        digits[5] = (char)('0' + (day / 10));
        digits[6] = (char)('0' + (day % 10));
        for (int i = 7; i < 12; i++)
            digits[i] = (char)('0' + rng.Next(10));
        digits[12] = (char)('0' + ComputeCheckDigit(digits[..12]));
        written = 13;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        for (int i = 0; i < Weights.Length; i++)
            sum += (digits[i] - '0') * Weights[i];
        int r = sum % 11;
        return r == 10 ? 1 : r;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-' || ch == '/') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}

