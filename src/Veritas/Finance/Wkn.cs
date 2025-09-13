using System;
using Veritas;

namespace Veritas.Finance;

public readonly struct WknValue
{
    public string Value { get; }
    public WknValue(string value) => Value = value;
}

public static class Wkn
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<WknValue> result)
    {
        Span<char> chars = stackalloc char[6];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<WknValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 6)
        {
            result = new ValidationResult<WknValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<WknValue>(true, new WknValue(new string(chars)), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            char c = char.ToUpperInvariant(ch);
            if (!char.IsLetterOrDigit(c)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = c;
        }
        return true;
    }
}
