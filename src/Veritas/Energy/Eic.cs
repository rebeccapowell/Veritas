using System;
using Veritas;

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
        // TODO: implement official checksum (mod 37) when spec available
        string value = new string(buffer);
        result = new ValidationResult<EicValue>(true, new EicValue(value), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int written)
    {
        written = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            char u = char.ToUpperInvariant(ch);
            if (!(char.IsDigit(u) || (u >= 'A' && u <= 'Z'))) { written = 0; return false; }
            if (written >= dest.Length) { written = 0; return false; }
            dest[written++] = u;
        }
        return true;
    }
}

