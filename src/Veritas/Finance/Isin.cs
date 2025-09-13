using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Finance;

/// <summary>Represents a validated ISIN.</summary>
public readonly struct IsinValue
{
    public string Value { get; }
    public IsinValue(string value) => Value = value;
}

/// <summary>Provides helpers for ISIN validation.</summary>
public static class Isin
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IsinValue> result)
    {
        Span<char> buffer = stackalloc char[12];
        if (!Normalize(input, buffer, out int len) || len != 12)
        {
            result = new ValidationResult<IsinValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> digits = stackalloc char[24];
        int idx = 0;
        for (int i = 0; i < len; i++)
        {
            char ch = buffer[i];
            if (ch >= 'A' && ch <= 'Z')
            {
                int v = ch - 'A' + 10;
                digits[idx++] = (char)('0' + v / 10);
                digits[idx++] = (char)('0' + v % 10);
            }
            else
            {
                digits[idx++] = ch;
            }
        }
        if (!Luhn.Validate(digits[..idx]))
        {
            result = new ValidationResult<IsinValue>(false, default, ValidationError.Checksum);
            return false;
        }
        string value = new string(buffer[..len]);
        result = new ValidationResult<IsinValue>(true, new IsinValue(value), ValidationError.None);
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

