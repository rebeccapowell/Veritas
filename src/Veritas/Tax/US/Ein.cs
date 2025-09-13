using System;
using Veritas;

namespace Veritas.Tax.US;

public readonly struct EinValue
{
    public string Value { get; }
    public EinValue(string value) => Value = value;
}

public static class Ein
{
    private static readonly int[] Prefixes = new[]
    {
        1,2,3,4,5,6,10,11,12,13,14,15,16,20,21,22,23,24,25,26,27,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,71,72,73,74,75,76,77,80,81,82,83,84,85,86,87,88,90,91,92,93,94,95,98,99
    };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EinValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<EinValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9)
        {
            result = new ValidationResult<EinValue>(false, default, ValidationError.Length);
            return false;
        }
        int prefix = (digits[0] - '0') * 10 + (digits[1] - '0');
        if (Array.BinarySearch(Prefixes, prefix) < 0)
        {
            result = new ValidationResult<EinValue>(false, default, ValidationError.CountryRule);
            return false;
        }
        string value = new string(digits);
        result = new ValidationResult<EinValue>(true, new EinValue(value), ValidationError.None);
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

