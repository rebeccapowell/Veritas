using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Logistics;

public readonly struct GsinValue { public string Value { get; } public GsinValue(string v) => Value = v; }

public static class Gsin
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<GsinValue> result)
    {
        Span<char> digits = stackalloc char[17];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<GsinValue>(false, default, ValidationError.Charset); return false; }
            if (len >= 17) { result = new ValidationResult<GsinValue>(false, default, ValidationError.Length); return false; }
            digits[len++] = ch;
        }
        if (len != 17) { result = new ValidationResult<GsinValue>(false, default, ValidationError.Length); return false; }
        if (!Gs1.Validate(digits)) { result = new ValidationResult<GsinValue>(false, default, ValidationError.Checksum); return false; }
        result = new ValidationResult<GsinValue>(true, new GsinValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 17)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 16; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[16] = (char)('0' + Gs1.ComputeCheckDigit(destination[..16]));
        written = 17;
        return true;
    }
}
