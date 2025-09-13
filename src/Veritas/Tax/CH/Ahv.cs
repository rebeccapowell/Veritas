using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Tax.CH;

public readonly struct AhvValue
{
    public string Value { get; }
    public AhvValue(string value) => Value = value;
}

/// <summary>
/// Swiss AHV/AVS number (13 digits, ISO 7064 mod 11,10).
/// </summary>
public static class Ahv
{

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<AhvValue> result)
    {
        Span<char> digits = stackalloc char[13];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<AhvValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 13)
        {
            result = new ValidationResult<AhvValue>(false, default, ValidationError.Length);
            return false;
        }
        if (!Iso7064.ValidateMod11_10(digits))
        {
            result = new ValidationResult<AhvValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<AhvValue>(true, new AhvValue(new string(digits)), ValidationError.None);
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
        digits[12] = Iso7064.ComputeCheckDigitMod11_10(digits[..12]);
        written = 13;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == '.' || ch == ' ' || ch == '-') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}

