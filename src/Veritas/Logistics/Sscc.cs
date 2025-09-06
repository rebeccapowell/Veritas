using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Logistics;

public readonly struct SsccValue { public string Value { get; } public SsccValue(string v) => Value = v; }

public static class Sscc
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<SsccValue> result)
    {
        Span<char> digits = stackalloc char[18];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<SsccValue>(false, default, ValidationError.Charset); return true; }
            if (len >= 18) { result = new ValidationResult<SsccValue>(false, default, ValidationError.Length); return true; }
            digits[len++] = ch;
        }
        if (len != 18) { result = new ValidationResult<SsccValue>(false, default, ValidationError.Length); return true; }
        if (!Gs1.Validate(digits)) { result = new ValidationResult<SsccValue>(false, default, ValidationError.Checksum); return true; }
        result = new ValidationResult<SsccValue>(true, new SsccValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 18)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 17; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[17] = (char)('0' + Gs1.ComputeCheckDigit(destination[..17]));
        written = 18;
        return true;
    }
}
