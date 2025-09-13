using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Identity.SouthAfrica;

public readonly struct NationalIdValue
{
    public string Value { get; }
    public NationalIdValue(string value) => Value = value;
}

/// <summary>
/// South Africa National ID (13 digits, Luhn checksum).
/// </summary>
public static class NationalId
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NationalIdValue> result)
    {
        Span<char> digits = stackalloc char[13];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<NationalIdValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 13)
        {
            result = new ValidationResult<NationalIdValue>(false, default, ValidationError.Length);
            return false;
        }
        if (!Luhn.Validate(digits))
        {
            result = new ValidationResult<NationalIdValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<NationalIdValue>(true, new NationalIdValue(new string(digits)), ValidationError.None);
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
        for (int i = 0; i < 12; i++)
            digits[i] = (char)('0' + rng.Next(10));
        digits[12] = (char)('0' + Luhn.ComputeCheckDigit(digits[..12]));
        written = 13;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (!char.IsDigit(ch) || len >= dest.Length)
            { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}
