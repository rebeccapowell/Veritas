using System;
using Veritas;

namespace Veritas.Tax.ES;

public readonly struct NieValue
{
    public string Value { get; }
    public NieValue(string value) => Value = value;
}

public static class Nie
{
    private const string Letters = "TRWAGMYFPDXBNJZSQVHLCKE";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NieValue> result)
    {
        Span<char> buffer = stackalloc char[9];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<NieValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 9)
        {
            result = new ValidationResult<NieValue>(false, default, ValidationError.Length);
            return true;
        }
        char first = buffer[0];
        if (first != 'X' && first != 'Y' && first != 'Z')
        {
            result = new ValidationResult<NieValue>(false, default, ValidationError.CountryRule);
            return true;
        }
        int number = first == 'X' ? 0 : first == 'Y' ? 1 : 2;
        for (int i = 1; i < 8; i++)
        {
            number = number * 10 + (buffer[i] - '0');
        }
        char expected = Letters[number % 23];
        if (buffer[8] != expected)
        {
            result = new ValidationResult<NieValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<NieValue>(true, new NieValue(new string(buffer)), ValidationError.None);
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

