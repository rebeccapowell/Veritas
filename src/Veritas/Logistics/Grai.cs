using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Logistics;

public readonly struct GraiValue { public string Value { get; } public GraiValue(string v) => Value = v; }

public static class Grai
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<GraiValue> result)
    {
        Span<char> digits = stackalloc char[14];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<GraiValue>(false, default, ValidationError.Charset); return false; }
            if (len >= 14) { result = new ValidationResult<GraiValue>(false, default, ValidationError.Length); return false; }
            digits[len++] = ch;
        }
        if (len != 14) { result = new ValidationResult<GraiValue>(false, default, ValidationError.Length); return false; }
        if (!Gs1.Validate(digits)) { result = new ValidationResult<GraiValue>(false, default, ValidationError.Checksum); return false; }
        result = new ValidationResult<GraiValue>(true, new GraiValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 14)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 13; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[13] = (char)('0' + Gs1.ComputeCheckDigit(destination[..13]));
        written = 14;
        return true;
    }
}
