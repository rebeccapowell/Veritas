using System;
using Veritas;

namespace Veritas.Tax.SK;

public readonly struct RodneCisloValue
{
    public string Value { get; }
    public RodneCisloValue(string value) => Value = value;
}

/// <summary>
/// Czech Rodné číslo (9 or 10 digits, mod-11 check).
/// </summary>
public static class RodneCislo
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<RodneCisloValue> result)
    {
        Span<char> digits = stackalloc char[10];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<RodneCisloValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 9 && len != 10)
        {
            result = new ValidationResult<RodneCisloValue>(false, default, ValidationError.Length);
            return true;
        }
        if (len == 9)
        {
            if (ParseLong(digits[..9]) % 11 != 0)
            {
                result = new ValidationResult<RodneCisloValue>(false, default, ValidationError.Checksum);
                return true;
            }
        }
        else
        {
            long baseNum = ParseLong(digits[..9]);
            int check = (int)(baseNum % 11);
            if (check == 10) check = 0;
            if (check != digits[9] - '0')
            {
                result = new ValidationResult<RodneCisloValue>(false, default, ValidationError.Checksum);
                return true;
            }
        }
        result = new ValidationResult<RodneCisloValue>(true, new RodneCisloValue(new string(digits[..len])), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 10) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int year = rng.Next(1900, 2100);
        int month = rng.Next(1, 13);
        int day = rng.Next(1, DateTime.DaysInMonth(year, month) + 1);
        int serial = rng.Next(0, 1000);
        Span<char> digits = destination;
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
        long baseNum = ParseLong(digits[..9]);
        int check = (int)(baseNum % 11);
        if (check == 10) check = 0;
        digits[9] = (char)('0' + check);
        written = 10;
        return true;
    }

    private static long ParseLong(ReadOnlySpan<char> digits)
    {
        long n = 0;
        foreach (var ch in digits) n = n*10 + (ch - '0');
        return n;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == '/' || ch == ' ') continue;
            if (!char.IsDigit(ch) || len >= dest.Length)
            {
                len = 0; return false;
            }
            dest[len++] = ch;
        }
        return true;
    }
}

