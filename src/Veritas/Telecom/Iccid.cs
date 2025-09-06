using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Telecom;

public readonly struct IccidValue { public string Value { get; } public IccidValue(string v) => Value = v; }

public static class Iccid
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IccidValue> result)
    {
        Span<char> digits = stackalloc char[20];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<IccidValue>(false, default, ValidationError.Charset); return true; }
            if (len >= 20) { result = new ValidationResult<IccidValue>(false, default, ValidationError.Length); return true; }
            digits[len++] = ch;
        }
        if (len < 19 || len > 20) { result = new ValidationResult<IccidValue>(false, default, ValidationError.Length); return true; }
        if (!Luhn.Validate(digits[..len])) { result = new ValidationResult<IccidValue>(false, default, ValidationError.Checksum); return true; }
        result = new ValidationResult<IccidValue>(true, new IccidValue(new string(digits[..len])), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        const int length = 20;
        if (destination.Length < length)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < length - 1; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[length - 1] = (char)('0' + Luhn.ComputeCheckDigit(destination[..(length - 1)]));
        written = length;
        return true;
    }
}
