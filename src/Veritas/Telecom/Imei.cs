using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Telecom;

public readonly struct ImeiValue { public string Value { get; } public ImeiValue(string v) => Value = v; }

public static class Imei
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ImeiValue> result)
    {
        Span<char> digits = stackalloc char[15];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<ImeiValue>(false, default, ValidationError.Charset); return true; }
            if (len >= 15) { result = new ValidationResult<ImeiValue>(false, default, ValidationError.Length); return true; }
            digits[len++] = ch;
        }
        if (len != 15) { result = new ValidationResult<ImeiValue>(false, default, ValidationError.Length); return true; }
        if (!Luhn.Validate(digits)) { result = new ValidationResult<ImeiValue>(false, default, ValidationError.Checksum); return true; }
        result = new ValidationResult<ImeiValue>(true, new ImeiValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 15)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 14; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[14] = (char)('0' + Luhn.ComputeCheckDigit(destination[..14]));
        written = 15;
        return true;
    }
}
