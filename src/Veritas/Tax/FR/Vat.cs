using System;
using Veritas;

namespace Veritas.Tax.FR;

public readonly struct VatValue
{
    public string Value { get; }
    public VatValue(string value) => Value = value;
}

public static class Vat
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<VatValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<VatValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 11)
        {
            result = new ValidationResult<VatValue>(false, default, ValidationError.Length);
            return false;
        }
        // first two digits are the checksum key
        int key = (digits[0] - '0') * 10 + (digits[1] - '0');
        int siren = 0;
        for (int i = 2; i < 11; i++)
        {
            siren = siren * 10 + (digits[i] - '0');
        }
        int expected = (12 + 3 * (siren % 97)) % 97;
        if (key != expected)
        {
            result = new ValidationResult<VatValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<VatValue>(true, new VatValue(new string(digits)), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        int i = 0;
        if (input.Length >= 2 && (input[0] == 'F' || input[0] == 'f') && (input[1] == 'R' || input[1] == 'r'))
            i = 2; // skip FR prefix
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

