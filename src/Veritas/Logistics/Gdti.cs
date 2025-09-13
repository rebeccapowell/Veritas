using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Logistics;

/// <summary>GS1 Global Document Type Identifier (GDTI).</summary>
public readonly struct GdtiValue
{
    /// <summary>The normalized GDTI string.</summary>
    public string Value { get; }
    public GdtiValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for GDTI codes.</summary>
public static class Gdti
{
    /// <summary>Validates the supplied GDTI.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<GdtiValue> result)
    {
        Span<char> digits = stackalloc char[14];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9' || len >= 14)
            {
                result = new ValidationResult<GdtiValue>(false, default, ValidationError.Charset);
                return false;
            }
            digits[len++] = ch;
        }
        if (len != 14)
        {
            result = new ValidationResult<GdtiValue>(false, default, ValidationError.Length);
            return false;
        }
        if (!Gs1.Validate(digits))
        {
            result = new ValidationResult<GdtiValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<GdtiValue>(true, new GdtiValue(new string(digits)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a valid GDTI into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a valid GDTI using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 14)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 13; i++) destination[i] = (char)('0' + rng.Next(10));
        destination[13] = (char)('0' + Gs1.ComputeCheckDigit(destination[..13]));
        written = 14;
        return true;
    }
}

