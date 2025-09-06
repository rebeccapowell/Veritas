using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Logistics;

public readonly struct GlnValue { public string Value { get; } public GlnValue(string v) => Value = v; }

public static class Gln
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<GlnValue> result)
    {
        Span<char> digits = stackalloc char[13];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<GlnValue>(false, default, ValidationError.Charset); return true; }
            if (len >= 13) { result = new ValidationResult<GlnValue>(false, default, ValidationError.Length); return true; }
            digits[len++] = ch;
        }
        if (len != 13) { result = new ValidationResult<GlnValue>(false, default, ValidationError.Length); return true; }
        if (!Gs1.Validate(digits)) { result = new ValidationResult<GlnValue>(false, default, ValidationError.Checksum); return true; }
        result = new ValidationResult<GlnValue>(true, new GlnValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 13)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 12; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[12] = (char)('0' + Gs1.ComputeCheckDigit(destination[..12]));
        written = 13;
        return true;
    }
}
