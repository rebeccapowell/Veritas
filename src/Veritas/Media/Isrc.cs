using System;
using Veritas;

namespace Veritas.Media;

/// <summary>Represents a validated International Standard Recording Code.</summary>
public readonly struct IsrcValue
{
    public string Value { get; }
    public IsrcValue(string value) => Value = value;
}

/// <summary>Provides structural validation for ISRC identifiers.</summary>
public static class Isrc
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IsrcValue> result)
    {
        Span<char> buf = stackalloc char[12];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == '-' || ch == ' ') continue;
            char up = char.ToUpperInvariant(ch);
            if (len >= 12) { result = new ValidationResult<IsrcValue>(false, default, ValidationError.Length); return true; }
            buf[len++] = up;
        }
        if (len != 12) { result = new ValidationResult<IsrcValue>(false, default, ValidationError.Length); return true; }
        // CC
        if (!IsLetter(buf[0]) || !IsLetter(buf[1])) { result = new ValidationResult<IsrcValue>(false, default, ValidationError.Charset); return true; }
        // Registrant (alnum)
        for (int i = 2; i < 5; i++)
            if (!IsAlnum(buf[i])) { result = new ValidationResult<IsrcValue>(false, default, ValidationError.Charset); return true; }
        // Year (digits)
        if (!IsDigit(buf[5]) || !IsDigit(buf[6])) { result = new ValidationResult<IsrcValue>(false, default, ValidationError.Charset); return true; }
        // Designation code (digits)
        for (int i = 7; i < 12; i++)
            if (!IsDigit(buf[i])) { result = new ValidationResult<IsrcValue>(false, default, ValidationError.Charset); return true; }
        result = new ValidationResult<IsrcValue>(true, new IsrcValue(new string(buf)), ValidationError.None);
        return true;
    }

    private static bool IsLetter(char c) => c >= 'A' && c <= 'Z';
    private static bool IsDigit(char c) => c >= '0' && c <= '9';
    private static bool IsAlnum(char c) => IsLetter(c) || IsDigit(c);
}
