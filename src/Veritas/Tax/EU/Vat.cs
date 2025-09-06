using System;
using Veritas;

namespace Veritas.Tax.EU;

public readonly struct VatValue
{
    public string Value { get; }
    public VatValue(string value) => Value = value;
}

public static class Vat
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<VatValue> result)
    {
        Span<char> buffer = stackalloc char[11];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<VatValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 11)
        {
            result = new ValidationResult<VatValue>(false, default, ValidationError.Length);
            return true;
        }
        if (buffer[0] != 'E' || buffer[1] != 'U')
        {
            result = new ValidationResult<VatValue>(false, default, ValidationError.CountryRule);
            return true;
        }
        for (int i = 2; i < 11; i++)
        {
            if (buffer[i] < '0' || buffer[i] > '9')
            {
                result = new ValidationResult<VatValue>(false, default, ValidationError.Charset);
                return true;
            }
        }
        result = new ValidationResult<VatValue>(true, new VatValue(new string(buffer)), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            if (len >= dest.Length) return false;
            if (ch >= 'a' && ch <= 'z') dest[len++] = (char)(ch - 32);
            else if (ch >= 'A' && ch <= 'Z') dest[len++] = ch;
            else if (ch >= '0' && ch <= '9') dest[len++] = ch;
            else return false;
        }
        return true;
    }
}
