using System;
using Veritas;

namespace Veritas.Tax.UK;

public readonly struct VatValue
{
    public string Value { get; }
    public VatValue(string value) => Value = value;
}

public static class Vat
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<VatValue> result)
    {
        Span<char> digits = stackalloc char[12];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<VatValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 9 && len != 12)
        {
            result = new ValidationResult<VatValue>(false, default, ValidationError.Length);
            return true;
        }
        if (!ValidateCore(digits[..9]))
        {
            result = new ValidationResult<VatValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<VatValue>(true, new VatValue(new string(digits[..len])), ValidationError.None);
        return true;
    }

    private static bool ValidateCore(ReadOnlySpan<char> digits)
    {
        int sum = (digits[0] - '0') * 8 + (digits[1] - '0') * 7 + (digits[2] - '0') * 6 +
                  (digits[3] - '0') * 5 + (digits[4] - '0') * 4 + (digits[5] - '0') * 3 +
                  (digits[6] - '0') * 2 + (digits[7] - '0') * 10 + (digits[8] - '0');
        int remainder = sum % 97;
        int prefix = (digits[0] - '0') * 100 + (digits[1] - '0') * 10 + (digits[2] - '0');
        if (prefix < 100)
            return remainder == 0;
        return remainder == 0 || remainder == 42 || remainder == 55;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        int i = 0;
        if (input.Length >= 2)
        {
            char c0 = input[0];
            char c1 = input[1];
            if ((c0 == 'G' || c0 == 'g' || c0 == 'X' || c0 == 'x') &&
                (c1 == 'B' || c1 == 'b' || c1 == 'I' || c1 == 'i'))
            {
                i = 2; // skip GB or XI
            }
        }
        for (; i < input.Length; i++)
        {
            char ch = input[i];
            if (ch == ' ' || ch == '\t') continue;
            if (ch < '0' || ch > '9') return false;
            if (len >= dest.Length) return false;
            dest[len++] = ch;
        }
        return true;
    }
}

