using System;
using Veritas;

namespace Veritas.Tax.NL;

public readonly struct BsnValue
{
    public string Value { get; }
    public BsnValue(string value) => Value = value;
}

public static class Bsn
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<BsnValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<BsnValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9)
        {
            result = new ValidationResult<BsnValue>(false, default, ValidationError.Length);
            return false;
        }
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            sum += (9 - i) * (digits[i] - '0');
        }
        sum += -1 * (digits[8] - '0');
        if (sum % 11 != 0)
        {
            result = new ValidationResult<BsnValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<BsnValue>(true, new BsnValue(new string(digits)), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            if (ch < '0' || ch > '9') return false;
            if (len >= dest.Length) return false;
            dest[len++] = ch;
        }
        return true;
    }
}

