using System;
using Veritas;

namespace Veritas.Finance;

public readonly struct MicValue
{
    public string Value { get; }
    public MicValue(string value) => Value = value;
}

public static class Mic
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MicValue> result)
    {
        Span<char> chars = stackalloc char[4];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<MicValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 4)
        {
            result = new ValidationResult<MicValue>(false, default, ValidationError.Length);
            return true;
        }
        result = new ValidationResult<MicValue>(true, new MicValue(new string(chars)), ValidationError.None);
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
