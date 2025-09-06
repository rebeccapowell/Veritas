using System;
using Veritas;

namespace Veritas.Energy.DE;

public readonly struct MaloValue
{
    public string Value { get; }
    public MaloValue(string value) => Value = value;
}

public static class Malo
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MaloValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len) || len != 11)
        {
            result = new ValidationResult<MaloValue>(false, default, ValidationError.Length);
            return true;
        }
        int sumOdd = 0, sumEven = 0;
        for (int i = 0; i < 10; i++)
        {
            int d = digits[i] - '0';
            if ((i & 1) == 0) sumOdd += d; else sumEven += d;
        }
        int total = sumOdd + 2 * sumEven;
        int check = (10 - total % 10) % 10;
        if (digits[10] != (char)('0' + check))
        {
            result = new ValidationResult<MaloValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(digits);
        result = new ValidationResult<MaloValue>(true, new MaloValue(value), ValidationError.None);
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
        for (int i = 1; i < 10; i++)
            digits[i] = (char)('0' + rng.Next(10));
        int sumOdd = 0, sumEven = 0;
        for (int i = 0; i < 10; i++)
        {
            int d = digits[i] - '0';
            if ((i & 1) == 0) sumOdd += d; else sumEven += d;
        }
        int total = sumOdd + 2 * sumEven;
        digits[10] = (char)('0' + ((10 - total % 10) % 10));
        written = 11;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}
