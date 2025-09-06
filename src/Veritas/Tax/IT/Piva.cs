using System;
using Veritas;

namespace Veritas.Tax.IT;

public readonly struct PivaValue
{
    public string Value { get; }
    public PivaValue(string value) => Value = value;
}

public static class Piva
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PivaValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<PivaValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 11)
        {
            result = new ValidationResult<PivaValue>(false, default, ValidationError.Length);
            return true;
        }
        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            int d = digits[i] - '0';
            if ((i & 1) == 0)
            {
                sum += d;
            }
            else
            {
                d *= 2;
                if (d > 9) d -= 9;
                sum += d;
            }
        }
        int check = (10 - (sum % 10)) % 10;
        if (check != digits[10] - '0')
        {
            result = new ValidationResult<PivaValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<PivaValue>(true, new PivaValue(new string(digits)), ValidationError.None);
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

