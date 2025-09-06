using System;
using Veritas;

namespace Veritas.Energy.IT;

public readonly struct PodValue
{
    public string Value { get; }
    public PodValue(string value) => Value = value;
}

public static class Pod
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PodValue> result)
    {
        Span<char> chars = stackalloc char[16];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<PodValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 16 || chars[0] != 'I' || chars[1] != 'T')
        {
            result = new ValidationResult<PodValue>(false, default, ValidationError.Format);
            return true;
        }
        result = new ValidationResult<PodValue>(true, new PodValue(new string(chars)), ValidationError.None);
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
