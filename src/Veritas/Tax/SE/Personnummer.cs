using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Tax.SE;

public readonly struct PersonnummerValue
{
    public string Value { get; }
    public PersonnummerValue(string value) => Value = value;
}

public static class Personnummer
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PersonnummerValue> result)
    {
        Span<char> digits = stackalloc char[12];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<PersonnummerValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 10 && len != 12)
        {
            result = new ValidationResult<PersonnummerValue>(false, default, ValidationError.Length);
            return true;
        }
        ReadOnlySpan<char> num = len == 10 ? digits[..10] : digits[(len - 10)..len];
        if (!IsValidDate(num))
        {
            result = new ValidationResult<PersonnummerValue>(false, default, ValidationError.Format);
            return true;
        }
        if (!Luhn.Validate(num))
        {
            result = new ValidationResult<PersonnummerValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<PersonnummerValue>(true, new PersonnummerValue(new string(digits[..len])), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t' || ch == '-' || ch == '+') continue;
            if (ch < '0' || ch > '9') return false;
            if (len >= dest.Length) return false;
            dest[len++] = ch;
        }
        return true;
    }

    private static bool IsValidDate(ReadOnlySpan<char> num)
    {
        int year = (num[0] - '0') * 10 + (num[1] - '0');
        int month = (num[2] - '0') * 10 + (num[3] - '0');
        int day = (num[4] - '0') * 10 + (num[5] - '0');
        if (month < 1 || month > 12 || day < 1) return false;
        // Year is relative; use 2000 as placeholder for leap-year logic
        try { var _ = new DateTime(2000 + year, month, day); }
        catch { return false; }
        return true;
    }
}
