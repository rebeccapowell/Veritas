using System;
using Veritas;

namespace Veritas.Energy.DE;

public readonly struct MeloValue
{
    public string Value { get; }
    public MeloValue(string value) => Value = value;
}

public static class Melo
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MeloValue> result)
    {
        Span<char> digits = stackalloc char[33];
        if (!Normalize(input, digits, out int len) || len != 33)
        {
            result = new ValidationResult<MeloValue>(false, default, ValidationError.Length);
            return true;
        }
        string value = new string(digits);
        result = new ValidationResult<MeloValue>(true, new MeloValue(value), ValidationError.None);
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
