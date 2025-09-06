using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Finance;

/// <summary>Represents a validated payment card PAN.</summary>
public readonly struct PanValue
{
    public string Value { get; }
    public PanValue(string value) => Value = value;
}

/// <summary>Provides validation for payment card PANs.</summary>
public static class Pan
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PanValue> result)
    {
        Span<char> buffer = stackalloc char[19];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<PanValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len < 12 || len > 19)
        {
            result = new ValidationResult<PanValue>(false, default, ValidationError.Length);
            return true;
        }
        if (!Luhn.Validate(buffer[..len]))
        {
            result = new ValidationResult<PanValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(buffer[..len]);
        result = new ValidationResult<PanValue>(true, new PanValue(value), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int written)
    {
        written = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t' || ch == '-') continue;
            if (!char.IsDigit(ch)) { written = 0; return false; }
            if (written >= dest.Length) { written = 0; return false; }
            dest[written++] = ch;
        }
        return true;
    }
}

