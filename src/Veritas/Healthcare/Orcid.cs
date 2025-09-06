using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Healthcare;

public readonly struct OrcidValue { public string Value { get; } public OrcidValue(string v) => Value = v; }

public static class Orcid
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<OrcidValue> result)
    {
        Span<char> digits = stackalloc char[16];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == '-' || ch == ' ') continue;
            char u = char.ToUpperInvariant(ch);
            if ((u < '0' || u > '9') && !(u == 'X' && len == 15)) { result = new ValidationResult<OrcidValue>(false, default, ValidationError.Charset); return true; }
            if (len >= 16) { result = new ValidationResult<OrcidValue>(false, default, ValidationError.Length); return true; }
            digits[len++] = u;
        }
        if (len != 16) { result = new ValidationResult<OrcidValue>(false, default, ValidationError.Length); return true; }
        if (!Iso7064.ValidateMod11_2(digits)) { result = new ValidationResult<OrcidValue>(false, default, ValidationError.Checksum); return true; }
        result = new ValidationResult<OrcidValue>(true, new OrcidValue(new string(digits)), ValidationError.None);
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
