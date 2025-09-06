using System;
using Veritas;

namespace Veritas.Tax.BR;

public readonly struct CpfValue
{
    public string Value { get; }
    public CpfValue(string value) => Value = value;
}

public static class Cpf
{
    private static readonly int[] Weights1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] Weights2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CpfValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<CpfValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 11)
        {
            result = new ValidationResult<CpfValue>(false, default, ValidationError.Length);
            return true;
        }
        if (AllEqual(digits))
        {
            result = new ValidationResult<CpfValue>(false, default, ValidationError.Format);
            return true;
        }
        int d1 = ComputeCheckDigit(digits[..9], Weights1);
        if (digits[9] - '0' != d1)
        {
            result = new ValidationResult<CpfValue>(false, default, ValidationError.Checksum);
            return true;
        }
        int d2 = ComputeCheckDigit(digits[..10], Weights2);
        if (digits[10] - '0' != d2)
        {
            result = new ValidationResult<CpfValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(digits);
        result = new ValidationResult<CpfValue>(true, new CpfValue(value), ValidationError.None);
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
        for (int i = 0; i < 9; i++)
            digits[i] = (char)('0' + rng.Next(10));
        if (AllEqual(digits[..9]))
            digits[8] = digits[8] == '9' ? '8' : '9';
        digits[9] = (char)('0' + ComputeCheckDigit(digits[..9], Weights1));
        digits[10] = (char)('0' + ComputeCheckDigit(digits[..10], Weights2));
        written = 11;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits, ReadOnlySpan<int> weights)
    {
        int sum = 0;
        for (int i = 0; i < weights.Length; i++)
            sum += (digits[i] - '0') * weights[i];
        int r = (sum * 10) % 11;
        return r == 10 ? 0 : r;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == '.' || ch == '-' || ch == ' ') continue;
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

