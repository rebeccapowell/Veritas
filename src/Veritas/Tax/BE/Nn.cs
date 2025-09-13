using System;
using Veritas;

namespace Veritas.Tax.BE;

public readonly struct NnValue
{
    public string Value { get; }
    public NnValue(string value) => Value = value;
}

/// <summary>
/// Belgium National Number (NN) identifier (11 digits, mod-97 checksum).
/// </summary>
public static class Nn
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NnValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<NnValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 11)
        {
            result = new ValidationResult<NnValue>(false, default, ValidationError.Length);
            return false;
        }
        long baseNumber = ParseLong(digits[..9]);
        int check = (digits[9] - '0') * 10 + (digits[10] - '0');
        if (!Verify(baseNumber, check))
        {
            result = new ValidationResult<NnValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<NnValue>(true, new NnValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 11) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int year = rng.Next(1900, 2100);
        int month = rng.Next(1, 13);
        int day = rng.Next(1, DateTime.DaysInMonth(year, month) + 1);
        int serial = rng.Next(0, 1000);
        Span<char> digits = destination[..11];
        int yy = year % 100;
        digits[0] = (char)('0' + yy / 10);
        digits[1] = (char)('0' + yy % 10);
        digits[2] = (char)('0' + month / 10);
        digits[3] = (char)('0' + month % 10);
        digits[4] = (char)('0' + day / 10);
        digits[5] = (char)('0' + day % 10);
        digits[6] = (char)('0' + serial / 100);
        digits[7] = (char)('0' + (serial / 10) % 10);
        digits[8] = (char)('0' + serial % 10);
        long baseNumber = ParseLong(digits[..9]);
        int check = year >= 2000
            ? 97 - (int)((2000000000L + baseNumber) % 97)
            : 97 - (int)(baseNumber % 97);
        if (check == 0) check = 97;
        digits[9] = (char)('0' + check / 10);
        digits[10] = (char)('0' + check % 10);
        written = 11;
        return true;
    }

    private static bool Verify(long baseNumber, int check)
    {
        int c1 = 97 - (int)(baseNumber % 97);
        if (c1 == 0) c1 = 97;
        int c2 = 97 - (int)((2000000000L + baseNumber) % 97);
        if (c2 == 0) c2 = 97;
        return check == c1 || check == c2;
    }

    private static long ParseLong(ReadOnlySpan<char> digits)
    {
        long n = 0;
        foreach (var ch in digits)
            n = n * 10 + (ch - '0');
        return n;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == '.' || ch == '-' || ch == ' ') continue;
            if (!char.IsDigit(ch) || len >= dest.Length)
            {
                len = 0;
                return false;
            }
            dest[len++] = ch;
        }
        return true;
    }
}

