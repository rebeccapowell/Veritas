using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Tax.FR;

public readonly struct SirenValue
{
    public string Value { get; }
    public SirenValue(string value) => Value = value;
}

public static class Siren
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<SirenValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<SirenValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9)
        {
            result = new ValidationResult<SirenValue>(false, default, ValidationError.Length);
            return false;
        }
        if (!Luhn.Validate(digits))
        {
            result = new ValidationResult<SirenValue>(false, default, ValidationError.Checksum);
            return false;
        }
        string value = new string(digits);
        result = new ValidationResult<SirenValue>(true, new SirenValue(value), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}

