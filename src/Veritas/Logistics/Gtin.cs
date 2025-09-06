using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Logistics;

public readonly struct GtinValue { public string Value { get; } public GtinValue(string v) => Value = v; }

public static class Gtin
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<GtinValue> result)
    {
        Span<char> digits = stackalloc char[18];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<GtinValue>(false, default, ValidationError.Charset); return true; }
            if (len >= digits.Length) { result = new ValidationResult<GtinValue>(false, default, ValidationError.Length); return true; }
            digits[len++] = ch;
        }
        if (!(len == 8 || len == 12 || len == 13 || len == 14)) { result = new ValidationResult<GtinValue>(false, default, ValidationError.Length); return true; }
        if (!Gs1.Validate(digits[..len])) { result = new ValidationResult<GtinValue>(false, default, ValidationError.Checksum); return true; }
        result = new ValidationResult<GtinValue>(true, new GtinValue(new string(digits[..len])), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(int length, Span<char> destination, out int written)
        => TryGenerate(length, default, destination, out written);

    public static bool TryGenerate(int length, in GenerationOptions options, Span<char> destination, out int written)
    {
        if (!(length == 8 || length == 12 || length == 13 || length == 14) || destination.Length < length)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < length - 1; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[length - 1] = (char)('0' + Gs1.ComputeCheckDigit(destination[..(length - 1)]));
        written = length;
        return true;
    }
}
