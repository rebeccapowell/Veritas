using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Finance;

/// <summary>Represents a validated International Bank Account Number.</summary>
public readonly struct IbanValue
{
    public string Value { get; }
    public IbanValue(string value) => Value = value;
}

/// <summary>Provides validation helpers for IBAN strings.</summary>
public static class Iban
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IbanValue> result)
    {
        Span<char> normalized = stackalloc char[34];
        if (!Normalize(input, normalized, out int len))
        {
            result = new ValidationResult<IbanValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len < 5 || normalized[0] < 'A' || normalized[0] > 'Z' || normalized[1] < 'A' || normalized[1] > 'Z' ||
            normalized[2] < '0' || normalized[2] > '9' || normalized[3] < '0' || normalized[3] > '9')
        {
            result = new ValidationResult<IbanValue>(false, default, ValidationError.Length);
            return false;
        }

        string? pattern = IbanRegistry.GetBbanPattern(normalized[..2]);
        if (pattern is null || !NationalFormat.Matches(normalized[4..len], pattern))
        {
            result = new ValidationResult<IbanValue>(false, default, ValidationError.Format);
            return false;
        }

        int checkDigits = (normalized[2] - '0') * 10 + normalized[3] - '0';
        if (checkDigits < 2 || checkDigits > 98)
        {
            result = new ValidationResult<IbanValue>(false, default, ValidationError.Checksum);
            return false;
        }
        Span<char> digits = stackalloc char[68];
        int idx = 0;
        for (int i = 4; i < len; i++) Append(digits, ref idx, normalized[i]);
        for (int i = 0; i < 4; i++) Append(digits, ref idx, normalized[i]);
        if (Iso7064.ComputeMod97(digits[..idx]) != 1)
        {
            result = new ValidationResult<IbanValue>(false, default, ValidationError.Checksum);
            return false;
        }
        string value = new string(normalized[..len]);
        result = new ValidationResult<IbanValue>(true, new IbanValue(value), ValidationError.None);
        return true;
    }

    private static void Append(Span<char> dest, ref int idx, char ch)
    {
        if (ch >= 'A' && ch <= 'Z')
        {
            int v = ch - 'A' + 10;
            dest[idx++] = (char)('0' + v / 10);
            dest[idx++] = (char)('0' + v % 10);
        }
        else
        {
            dest[idx++] = ch;
        }
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int written)
    {
        written = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            char u = char.ToUpperInvariant(ch);
            if (!((u >= '0' && u <= '9') || (u >= 'A' && u <= 'Z'))) { written = 0; return false; }
            if (written >= dest.Length) { written = 0; return false; }
            dest[written++] = u;
        }
        return true;
    }
}

