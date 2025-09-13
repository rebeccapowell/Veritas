using System;
using Veritas;

namespace Veritas.Tax.PL;

public readonly struct NipValue
{
    public string Value { get; }
    public NipValue(string value) => Value = value;
}

public static class Nip
{
    private static readonly int[] Weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NipValue> result)
    {
        Span<char> digits = stackalloc char[10];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<NipValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 10)
        {
            result = new ValidationResult<NipValue>(false, default, ValidationError.Length);
            return false;
        }
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (digits[i] - '0') * Weights[i];
        int check = sum % 11;
        if (check == 10 || check != digits[9] - '0')
        {
            result = new ValidationResult<NipValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<NipValue>(true, new NipValue(new string(digits)), ValidationError.None);
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
