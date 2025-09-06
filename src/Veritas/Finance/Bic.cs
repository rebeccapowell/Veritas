using System;
using Veritas;

namespace Veritas.Finance;

/// <summary>Represents a validated BIC/SWIFT code.</summary>
public readonly struct BicValue
{
    public string Value { get; }
    public BicValue(string value) => Value = value;
}

/// <summary>Provides validation for BIC/SWIFT codes.</summary>
public static class Bic
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<BicValue> result)
    {
        Span<char> buffer = stackalloc char[11];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<BicValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 8 && len != 11)
        {
            result = new ValidationResult<BicValue>(false, default, ValidationError.Length);
            return true;
        }
        for (int i = 0; i < 4; i++)
        {
            var c = buffer[i];
            if (c < 'A' || c > 'Z')
            {
                result = new ValidationResult<BicValue>(false, default, ValidationError.Charset);
                return true;
            }
        }
        for (int i = 4; i < 6; i++)
        {
            var c = buffer[i];
            if (c < 'A' || c > 'Z')
            {
                result = new ValidationResult<BicValue>(false, default, ValidationError.Charset);
                return true;
            }
        }
        for (int i = 6; i < len; i++)
        {
            var c = buffer[i];
            if (!char.IsLetterOrDigit(c))
            {
                result = new ValidationResult<BicValue>(false, default, ValidationError.Charset);
                return true;
            }
        }
        string value = new string(buffer[..len]);
        result = new ValidationResult<BicValue>(true, new BicValue(value), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int written)
    {
        written = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            char u = char.ToUpperInvariant(ch);
            if (!char.IsLetterOrDigit(u)) { written = 0; return false; }
            if (written >= dest.Length) { written = 0; return false; }
            dest[written++] = u;
        }
        return true;
    }
}

