using System;
using Veritas;

namespace Veritas.Tax.TR;

public readonly struct TcknValue
{
    public string Value { get; }
    public TcknValue(string value) => Value = value;
}

/// <summary>
/// Turkey T.C. Kimlik number (11 digits, parity checks).
/// </summary>
public static class Tckn
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<TcknValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<TcknValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 11 || digits[0] == '0')
        {
            result = new ValidationResult<TcknValue>(false, default, ValidationError.Length);
            return false;
        }
        int oddSum = 0, evenSum = 0;
        for (int i = 0; i < 9; i++)
        {
            int d = digits[i] - '0';
            if ((i % 2) == 0) oddSum += d; else evenSum += d;
        }
        int d10 = ((oddSum * 7) - evenSum) % 10;
        if (d10 < 0) d10 += 10;
        if (digits[9] - '0' != d10)
        {
            result = new ValidationResult<TcknValue>(false, default, ValidationError.Checksum);
            return false;
        }
        int sumAll = oddSum + evenSum + d10;
        if (digits[10] - '0' != sumAll % 10)
        {
            result = new ValidationResult<TcknValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<TcknValue>(true, new TcknValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 11) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..11];
        digits[0] = (char)('1' + rng.Next(9));
        for (int i = 1; i < 9; i++) digits[i] = (char)('0' + rng.Next(10));
        int oddSum = 0, evenSum = 0;
        for (int i = 0; i < 9; i++)
        {
            int d = digits[i] - '0';
            if ((i % 2) == 0) oddSum += d; else evenSum += d;
        }
        int d10 = ((oddSum * 7) - evenSum) % 10; if (d10 < 0) d10 += 10;
        digits[9] = (char)('0' + d10);
        int sumAll = oddSum + evenSum + d10;
        digits[10] = (char)('0' + (sumAll % 10));
        written = 11;
        return true;
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

