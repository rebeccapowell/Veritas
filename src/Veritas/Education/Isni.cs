using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Education;

public readonly struct IsniValue { public string Value { get; } public IsniValue(string v) => Value = v; }

public static class Isni
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IsniValue> result)
    {
        Span<char> buf = stackalloc char[16];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char up = char.ToUpperInvariant(ch);
            if (len >= 16)
            {
                result = new ValidationResult<IsniValue>(false, default, ValidationError.Length);
                return false;
            }
            if ((up < '0' || up > '9') && up != 'X')
            {
                result = new ValidationResult<IsniValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[len++] = up;
        }
        if (len != 16)
        {
            result = new ValidationResult<IsniValue>(false, default, ValidationError.Length);
            return false;
        }
        if (!Iso7064.ValidateMod11_2(buf))
        {
            result = new ValidationResult<IsniValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<IsniValue>(true, new IsniValue(new string(buf)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 16)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 15; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[15] = Iso7064.ComputeCheckDigitMod11_2(destination[..15]);
        written = 16;
        return true;
    }
}

