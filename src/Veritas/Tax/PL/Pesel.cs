using System;
using Veritas;

namespace Veritas.Tax.PL;

public readonly struct PeselValue
{
    public string Value { get; }
    public PeselValue(string value) => Value = value;
}

public static class Pesel
{
    private static readonly int[] Weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PeselValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<PeselValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 11)
        {
            result = new ValidationResult<PeselValue>(false, default, ValidationError.Length);
            return true;
        }
        if (!IsValidDate(digits))
        {
            result = new ValidationResult<PeselValue>(false, default, ValidationError.Format);
            return true;
        }
        int sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (digits[i] - '0') * Weights[i];
        int check = (10 - (sum % 10)) % 10;
        if (check != digits[10] - '0')
        {
            result = new ValidationResult<PeselValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<PeselValue>(true, new PeselValue(new string(digits)), ValidationError.None);
        return true;
    }

    private static bool IsValidDate(ReadOnlySpan<char> digits)
    {
        int year = (digits[0] - '0') * 10 + (digits[1] - '0');
        int month = (digits[2] - '0') * 10 + (digits[3] - '0');
        int day = (digits[4] - '0') * 10 + (digits[5] - '0');
        int century;
        if (month >= 81) { century = 1800; month -= 80; }
        else if (month >= 61) { century = 2200; month -= 60; }
        else if (month >= 41) { century = 2100; month -= 40; }
        else if (month >= 21) { century = 2000; month -= 20; }
        else { century = 1900; }
        year += century;
        if (month < 1 || month > 12 || day < 1) return false;
        try { var d = new DateTime(year, month, day); } catch { return false; }
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t' || ch == '-' || ch == '/') continue;
            if (ch < '0' || ch > '9') return false;
            if (len >= dest.Length) return false;
            dest[len++] = ch;
        }
        return true;
    }
}
