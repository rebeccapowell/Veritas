using System;
using Veritas;

namespace Veritas.Tax.CO;

public readonly struct NitValue
{
    public string Value { get; }
    public NitValue(string value) => Value = value;
}

/// <summary>
/// Colombia NIT tax identifier (up to 10 digits, mod-11 checksum).
/// </summary>
public static class Nit
{
    private static readonly int[] Weights = { 3, 7, 13, 17, 19, 23, 29, 37, 41, 43, 47 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NitValue> result)
    {
        Span<char> digits = stackalloc char[20];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<NitValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len < 2 || len > 10)
        {
            result = new ValidationResult<NitValue>(false, default, ValidationError.Length);
            return false;
        }
        int check = ComputeCheckDigit(digits[..(len - 1)]);
        if (digits[len - 1] - '0' != check)
        {
            result = new ValidationResult<NitValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<NitValue>(true, new NitValue(new string(digits[..len])), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 10) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int len = rng.Next(2, 10); // including check
        Span<char> digits = destination[..len];
        for (int i = 0; i < len - 1; i++) digits[i] = (char)('0' + rng.Next(10));
        digits[len - 1] = (char)('0' + ComputeCheckDigit(digits[..(len - 1)]));
        written = len;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        int offset = Weights.Length - digits.Length;
        for (int i = 0; i < digits.Length; i++)
            sum += (digits[i] - '0') * Weights[offset + i];
        int r = sum % 11;
        return r < 2 ? 0 : 11 - r;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == '-' || ch == ' ') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}

