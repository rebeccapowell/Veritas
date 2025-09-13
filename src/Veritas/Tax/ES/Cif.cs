using System;
using Veritas;

namespace Veritas.Tax.ES;

public readonly struct CifValue
{
    public string Value { get; }
    public CifValue(string value) => Value = value;
}

public static class Cif
{
    private const string Letters = "JABCDEFGHI";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CifValue> result)
    {
        Span<char> buffer = stackalloc char[9];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<CifValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9)
        {
            result = new ValidationResult<CifValue>(false, default, ValidationError.Length);
            return false;
        }
        char first = buffer[0];
        int sum = 0;
        for (int i = 1; i < 8; i++)
        {
            int d = buffer[i] - '0';
            if ((i & 1) == 1) // odd positions (1-based)
            {
                int t = d * 2;
                sum += t / 10 + t % 10;
            }
            else
            {
                sum += d;
            }
        }
        int check = (10 - (sum % 10)) % 10;
        char digitChar = (char)('0' + check);
        char letterChar = Letters[check];
        char last = buffer[8];
        bool ok = first switch
        {
            'A' or 'B' or 'E' or 'H' => last == digitChar,
            'K' or 'P' or 'Q' or 'S' => last == letterChar,
            _ => last == digitChar || last == letterChar
        };
        if (!ok)
        {
            result = new ValidationResult<CifValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<CifValue>(true, new CifValue(new string(buffer)), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            if (len >= dest.Length) return false;
            if (ch >= '0' && ch <= '9') dest[len++] = ch;
            else if (ch >= 'a' && ch <= 'z') dest[len++] = (char)(ch - 32);
            else if (ch >= 'A' && ch <= 'Z') dest[len++] = ch;
            else return false;
        }
        return true;
    }
}

