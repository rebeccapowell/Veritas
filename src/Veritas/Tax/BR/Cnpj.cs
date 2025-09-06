using System;
using Veritas;

namespace Veritas.Tax.BR;

public readonly struct CnpjValue
{
    public string Value { get; }
    public CnpjValue(string value) => Value = value;
}

public static class Cnpj
{
    private static readonly int[] Weights1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] Weights2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CnpjValue> result)
    {
        Span<char> digits = stackalloc char[14];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<CnpjValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 14)
        {
            result = new ValidationResult<CnpjValue>(false, default, ValidationError.Length);
            return true;
        }
        if (AllEqual(digits))
        {
            result = new ValidationResult<CnpjValue>(false, default, ValidationError.Format);
            return true;
        }
        int d1 = ComputeCheckDigit(digits[..12], Weights1);
        if (digits[12] - '0' != d1)
        {
            result = new ValidationResult<CnpjValue>(false, default, ValidationError.Checksum);
            return true;
        }
        int d2 = ComputeCheckDigit(digits[..13], Weights2);
        if (digits[13] - '0' != d2)
        {
            result = new ValidationResult<CnpjValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(digits);
       result = new ValidationResult<CnpjValue>(true, new CnpjValue(value), ValidationError.None);
       return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 14) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..14];
        for (int i = 0; i < 12; i++)
            digits[i] = (char)('0' + rng.Next(10));
        if (AllEqual(digits[..12]))
            digits[11] = digits[11] == '9' ? '8' : '9';
        digits[12] = (char)('0' + ComputeCheckDigit(digits[..12], Weights1));
        digits[13] = (char)('0' + ComputeCheckDigit(digits[..13], Weights2));
        written = 14;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits, ReadOnlySpan<int> weights)
    {
        int sum = 0;
        for (int i = 0; i < weights.Length; i++)
            sum += (digits[i] - '0') * weights[i];
        int r = sum % 11;
        return r < 2 ? 0 : 11 - r;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == '.' || ch == '-' || ch == '/' || ch == ' ') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }

    private static bool AllEqual(ReadOnlySpan<char> digits)
    {
        for (int i = 1; i < digits.Length; i++)
            if (digits[i] != digits[0]) return false;
        return true;
    }
}

