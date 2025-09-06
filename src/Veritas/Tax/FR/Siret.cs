using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Tax.FR;

public readonly struct SiretValue
{
    public string Value { get; }
    public SiretValue(string value) => Value = value;
}

public static class Siret
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<SiretValue> result)
    {
        Span<char> digits = stackalloc char[14];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<SiretValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 14)
        {
            result = new ValidationResult<SiretValue>(false, default, ValidationError.Length);
            return true;
        }
        ReadOnlySpan<char> laPoste = "356000000";
        bool special = digits[..9].SequenceEqual(laPoste) && !digits.SequenceEqual("35600000000048");
        if (special)
        {
            int sum = 0;
            for (int i = 0; i < 14; i++) sum += digits[i] - '0';
            if (sum % 5 != 0)
            {
                result = new ValidationResult<SiretValue>(false, default, ValidationError.Checksum);
                return true;
            }
        }
        else if (!Luhn.Validate(digits))
        {
            result = new ValidationResult<SiretValue>(false, default, ValidationError.Checksum);
            return true;
        }
        if (!Siren.TryValidate(digits[..9], out var sr) || !sr.IsValid)
        {
            result = new ValidationResult<SiretValue>(false, default, ValidationError.CountryRule);
            return true;
        }
        string value = new string(digits);
        result = new ValidationResult<SiretValue>(true, new SiretValue(value), ValidationError.None);
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

