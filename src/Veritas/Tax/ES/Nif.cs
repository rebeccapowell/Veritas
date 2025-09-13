using System;
using Veritas;

namespace Veritas.Tax.ES;

public readonly struct NifValue
{
    public string Value { get; }
    public NifValue(string value) => Value = value;
}

public static class Nif
{
    private const string Letters = "TRWAGMYFPDXBNJZSQVHLCKE";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NifValue> result)
    {
        Span<char> buffer = stackalloc char[9];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<NifValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9)
        {
            result = new ValidationResult<NifValue>(false, default, ValidationError.Length);
            return false;
        }
        int number = 0;
        for (int i = 0; i < 8; i++)
        {
            number = number * 10 + (buffer[i] - '0');
        }
        char expected = Letters[number % 23];
        if (buffer[8] != expected)
        {
            result = new ValidationResult<NifValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<NifValue>(true, new NifValue(new string(buffer)), ValidationError.None);
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

