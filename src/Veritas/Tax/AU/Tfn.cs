using System;
using Veritas;

namespace Veritas.Tax.AU;

public readonly struct TfnValue
{
    public string Value { get; }
    public TfnValue(string value) => Value = value;
}

public static class Tfn
{
    private static readonly int[] Weights8 = { 1, 4, 3, 7, 5, 8, 6, 9 };
    private static readonly int[] Weights9 = { 1, 4, 3, 7, 5, 8, 6, 9, 10 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<TfnValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<TfnValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 8 && len != 9)
        {
            result = new ValidationResult<TfnValue>(false, default, ValidationError.Length);
            return false;
        }
        var weights = len == 8 ? Weights8 : Weights9;
        int sum = 0;
        for (int i = 0; i < len; i++)
            sum += (digits[i] - '0') * weights[i];
        if (sum % 11 != 0)
        {
            result = new ValidationResult<TfnValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<TfnValue>(true, new TfnValue(new string(digits[..len])), ValidationError.None);
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
        while (true)
        {
            for (int i = 0; i < 8; i++)
                digits[i] = (char)('0' + rng.Next(10));
            int sum = 0;
            for (int i = 0; i < 8; i++)
                sum += (digits[i] - '0') * Weights9[i];
            int check = ((11 - (sum % 11)) * 10) % 11;
            if (check == 10) continue;
            digits[8] = (char)('0' + check);
            written = 9;
            return true;
        }
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
