using System;
using Veritas;

namespace Veritas.Energy.DE;

public readonly struct MaloValue
{
    public string Value { get; }
    public MaloValue(string value) => Value = value;
}

public static class Malo
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MaloValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len) || len != 11)
        {
            result = new ValidationResult<MaloValue>(false, default, ValidationError.Length);
            return true;
        }
        string value = new string(digits);
        result = new ValidationResult<MaloValue>(true, new MaloValue(value), ValidationError.None);
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
