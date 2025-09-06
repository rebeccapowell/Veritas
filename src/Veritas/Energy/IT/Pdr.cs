using System;
using Veritas;

namespace Veritas.Energy.IT;

public readonly struct PdrValue
{
    public string Value { get; }
    public PdrValue(string value) => Value = value;
}

public static class Pdr
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PdrValue> result)
    {
        Span<char> digits = stackalloc char[14];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<PdrValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 14)
        {
            result = new ValidationResult<PdrValue>(false, default, ValidationError.Length);
            return true;
        }
        result = new ValidationResult<PdrValue>(true, new PdrValue(new string(digits)), ValidationError.None);
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
