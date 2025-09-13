using System;
using Veritas;

namespace Veritas.Tax.PL;

public readonly struct RegonValue
{
    public string Value { get; }
    public RegonValue(string value) => Value = value;
}

public static class Regon
{
    private static readonly int[] Weights9 = { 8, 9, 2, 3, 4, 5, 6, 7 };
    private static readonly int[] Weights14 = { 2, 3, 4, 5, 6, 7, 8, 9, 2, 3, 4, 5, 6, 7 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<RegonValue> result)
    {
        Span<char> digits = stackalloc char[14];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<RegonValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9 && len != 14)
        {
            result = new ValidationResult<RegonValue>(false, default, ValidationError.Length);
            return false;
        }
        if (len == 9)
        {
            int sum = 0;
            for (int i = 0; i < 8; i++)
                sum += (digits[i] - '0') * Weights9[i];
            int check = sum % 11;
            if (check == 10) check = 0;
            if (check != digits[8] - '0')
            {
                result = new ValidationResult<RegonValue>(false, default, ValidationError.Checksum);
                return false;
            }
        }
        else
        {
            int sum = 0;
            for (int i = 0; i < 13; i++)
                sum += (digits[i] - '0') * Weights14[i];
            int check = sum % 11;
            if (check == 10) check = 0;
            if (check != digits[13] - '0')
            {
                result = new ValidationResult<RegonValue>(false, default, ValidationError.Checksum);
                return false;
            }
        }
        result = new ValidationResult<RegonValue>(true, new RegonValue(new string(digits[..len])), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t' || ch == '-' || ch == '.') continue;
            if (ch < '0' || ch > '9') return false;
            if (len >= dest.Length) return false;
            dest[len++] = ch;
        }
        return true;
    }
}
