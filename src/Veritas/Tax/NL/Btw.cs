using System;
using Veritas;

namespace Veritas.Tax.NL;

public readonly struct BtwValue
{
    public string Value { get; }
    public BtwValue(string value) => Value = value;
}

public static class Btw
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<BtwValue> result)
    {
        Span<char> buffer = stackalloc char[12];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<BtwValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 12 || buffer[9] != 'B')
        {
            result = new ValidationResult<BtwValue>(false, default, ValidationError.Length);
            return true;
        }
        for (int i = 0; i < 9; i++)
        {
            if (buffer[i] < '0' || buffer[i] > '9')
            {
                result = new ValidationResult<BtwValue>(false, default, ValidationError.Format);
                return true;
            }
        }
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            sum += (9 - i) * (buffer[i] - '0');
        }
        int check = 11 - (sum % 11);
        if (check == 10 || check == 11) check = 0;
        if (check != buffer[8] - '0')
        {
            result = new ValidationResult<BtwValue>(false, default, ValidationError.Checksum);
            return true;
        }
        // last two digits after B are extension; any digits ok
        if (buffer[10] < '0' || buffer[10] > '9' || buffer[11] < '0' || buffer[11] > '9')
        {
            result = new ValidationResult<BtwValue>(false, default, ValidationError.Format);
            return true;
        }
        result = new ValidationResult<BtwValue>(true, new BtwValue(new string(buffer)), ValidationError.None);
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
            else if (ch == 'B' || ch == 'b') dest[len++] = 'B';
            else return false;
        }
        return true;
    }
}

