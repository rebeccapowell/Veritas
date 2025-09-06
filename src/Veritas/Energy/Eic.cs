using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Energy;

/// <summary>Represents a validated Energy Identification Code (EIC).</summary>
public readonly struct EicValue
{
    public string Value { get; }
    public EicValue(string value) => Value = value;
}

/// <summary>Provides validation for EIC codes.</summary>
public static class Eic
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EicValue> result)
    {
        Span<char> buffer = stackalloc char[16];
        if (!Normalize(input, buffer, out int len) || len != 16)
        {
            result = new ValidationResult<EicValue>(false, default, ValidationError.Length);
            return true;
        }
        if (!Iso7064.ValidateMod37_2(buffer))
        {
            result = new ValidationResult<EicValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(buffer);
        result = new ValidationResult<EicValue>(true, new EicValue(value), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 16) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> chars = destination[..16];
        for (int i = 0; i < 15; i++)
        {
            int v = rng.Next(36);
            chars[i] = v < 10 ? (char)('0' + v) : (char)('A' + v - 10);
        }
        chars[15] = Iso7064.ComputeCheckCharacterMod37_2(chars[..15]);
        written = 16;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int written)
    {
        written = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            char u = char.ToUpperInvariant(ch);
            if (!(char.IsDigit(u) || (u >= 'A' && u <= 'Z') || u == '*')) { written = 0; return false; }
            if (written >= dest.Length) { written = 0; return false; }
            dest[written++] = u;
        }
        return true;
    }
}

