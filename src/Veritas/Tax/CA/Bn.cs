using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Tax.CA;

public readonly struct BnValue
{
    public string Value { get; }
    public BnValue(string value) => Value = value;
}

public static class Bn
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<BnValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<BnValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 9)
        {
            result = new ValidationResult<BnValue>(false, default, ValidationError.Length);
            return true;
        }
        if (!Luhn.Validate(digits))
        {
            result = new ValidationResult<BnValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<BnValue>(true, new BnValue(new string(digits)), ValidationError.None);
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
        digits[8] = (char)('0' + Luhn.ComputeCheckDigit(digits[..8]));
        written = 9;
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
