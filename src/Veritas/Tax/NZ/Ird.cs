using System;
using Veritas;

namespace Veritas.Tax.NZ;

public readonly struct IrdValue
{
    public string Value { get; }
    public IrdValue(string value) => Value = value;
}

public static class Ird
{
    private static readonly int[] PrimaryWeights = { 3, 2, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] SecondaryWeights = { 7, 4, 3, 2, 5, 2, 7, 6 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IrdValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<IrdValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len < 8 || len > 9)
        {
            result = new ValidationResult<IrdValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> padded = stackalloc char[9];
        int offset = 9 - len;
        for (int i = 0; i < offset; i++) padded[i] = '0';
        digits[..len].CopyTo(padded[offset..]);
        int check = ComputeCheckDigit(padded, PrimaryWeights);
        if (check == 10)
            check = ComputeCheckDigit(padded, SecondaryWeights);
        if (check == 10 || padded[8] - '0' != check)
        {
            result = new ValidationResult<IrdValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<IrdValue>(true, new IrdValue(new string(padded)), ValidationError.None);
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits, ReadOnlySpan<int> weights)
    {
        int sum = 0;
        for (int i = 0; i < 8; i++)
            sum += (digits[i] - '0') * weights[i];
        int remainder = sum % 11;
        if (remainder == 0) return 0;
        int check = 11 - remainder;
        return check;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == '-') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}
