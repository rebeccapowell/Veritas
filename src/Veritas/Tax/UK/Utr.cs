using System;
using Veritas;

namespace Veritas.Tax.UK;

public readonly struct UtrValue
{
    public string Value { get; }
    public UtrValue(string value) => Value = value;
}

public static class Utr
{
    private static readonly int[] Weights = { 6, 7, 8, 9, 10, 5, 4, 3, 2 };
    private const string Map = "21987654321";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<UtrValue> result)
    {
        Span<char> digits = stackalloc char[10];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<UtrValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 10)
        {
            result = new ValidationResult<UtrValue>(false, default, ValidationError.Length);
            return true;
        }
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (digits[i + 1] - '0') * Weights[i];
        char check = Map[sum % 11];
        if (digits[0] != check)
        {
            result = new ValidationResult<UtrValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(digits);
        result = new ValidationResult<UtrValue>(true, new UtrValue(value), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}

