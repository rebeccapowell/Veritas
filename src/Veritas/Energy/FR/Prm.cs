using System;
using Veritas;

namespace Veritas.Energy.FR;

public readonly struct PrmValue
{
    public string Value { get; }
    public PrmValue(string value) => Value = value;
}

public static class Prm
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PrmValue> result)
    {
        Span<char> digits = stackalloc char[14];
        if (!Normalize(input, digits, out int len) || len != 14)
        {
            result = new ValidationResult<PrmValue>(false, default, ValidationError.Length);
            return true;
        }
        string value = new string(digits);
        result = new ValidationResult<PrmValue>(true, new PrmValue(value), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}
