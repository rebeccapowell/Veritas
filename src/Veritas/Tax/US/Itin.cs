using System;
using Veritas;

namespace Veritas.Tax.US;

public readonly struct ItinValue
{
    public string Value { get; }
    public ItinValue(string value) => Value = value;
}

public static class Itin
{
    private static readonly bool[] AllowedGroups = InitGroups();

    private static bool[] InitGroups()
    {
        var arr = new bool[100];
        for (int i = 70; i < 100; i++)
            if (i != 89 && i != 93) arr[i] = true;
        return arr;
    }

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ItinValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<ItinValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9)
        {
            result = new ValidationResult<ItinValue>(false, default, ValidationError.Length);
            return false;
        }
        if (digits[0] != '9')
        {
            result = new ValidationResult<ItinValue>(false, default, ValidationError.CountryRule);
            return false;
        }
        int group = (digits[3] - '0') * 10 + (digits[4] - '0');
        if (!AllowedGroups[group])
        {
            result = new ValidationResult<ItinValue>(false, default, ValidationError.CountryRule);
            return false;
        }
        string value = new string(digits);
        result = new ValidationResult<ItinValue>(true, new ItinValue(value), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-' || ch == '\t') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}

