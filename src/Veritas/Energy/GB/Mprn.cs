using System;
using Veritas;

namespace Veritas.Energy.GB;

public readonly struct MprnValue
{
    public string Value { get; }
    public MprnValue(string value) => Value = value;
}

public static class Mprn
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MprnValue> result)
    {
        Span<char> digits = stackalloc char[10];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<MprnValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len < 6 || len > 10)
        {
            result = new ValidationResult<MprnValue>(false, default, ValidationError.Length);
            return true;
        }
        string value = new string(digits[..len]);
        result = new ValidationResult<MprnValue>(true, new MprnValue(value), ValidationError.None);
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
