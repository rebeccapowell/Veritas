using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Tax.SE;

public readonly struct OrgNrValue
{
    public string Value { get; }
    public OrgNrValue(string value) => Value = value;
}

public static class OrgNr
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<OrgNrValue> result)
    {
        Span<char> digits = stackalloc char[12];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<OrgNrValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 10 && len != 12)
        {
            result = new ValidationResult<OrgNrValue>(false, default, ValidationError.Length);
            return false;
        }
        ReadOnlySpan<char> num = len == 10 ? digits[..10] : digits[(len - 10)..len];
        if (num[2] < '2')
        {
            result = new ValidationResult<OrgNrValue>(false, default, ValidationError.CountryRule);
            return false;
        }
        if (!Luhn.Validate(num))
        {
            result = new ValidationResult<OrgNrValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<OrgNrValue>(true, new OrgNrValue(new string(digits[..len])), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t' || ch == '-') continue;
            if (ch < '0' || ch > '9') return false;
            if (len >= dest.Length) return false;
            dest[len++] = ch;
        }
        return true;
    }
}
