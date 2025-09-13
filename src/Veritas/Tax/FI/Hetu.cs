using System;
using Veritas;

namespace Veritas.Tax.FI;

public readonly struct HetuValue
{
    public string Value { get; }
    public HetuValue(string value) => Value = value;
}

/// <summary>
/// Finland HETU personal identity code (11 characters, mod-31 check character).
/// </summary>
public static class Hetu
{
    private const string CheckMap = "0123456789ABCDEFHJKLMNPRSTUVWXY";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<HetuValue> result)
    {
        Span<char> buffer = stackalloc char[11];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<HetuValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 11)
        {
            result = new ValidationResult<HetuValue>(false, default, ValidationError.Length);
            return true;
        }
        char sep = buffer[6];
        if (sep != '+' && sep != '-' && sep != 'A')
        {
            result = new ValidationResult<HetuValue>(false, default, ValidationError.Format);
            return true;
        }
        for (int i = 0; i < 6; i++)
            if (!char.IsDigit(buffer[i]))
            {
                result = new ValidationResult<HetuValue>(false, default, ValidationError.Format);
                return true;
            }
        for (int i = 7; i < 10; i++)
            if (!char.IsDigit(buffer[i]))
            {
                result = new ValidationResult<HetuValue>(false, default, ValidationError.Format);
                return true;
            }
        char check = char.ToUpperInvariant(buffer[10]);
        if (!CheckMap.Contains(check))
        {
            result = new ValidationResult<HetuValue>(false, default, ValidationError.Format);
            return true;
        }
        int number = 0;
        for (int i = 0; i < 6; i++)
            number = number * 10 + (buffer[i] - '0');
        for (int i = 7; i < 10; i++)
            number = number * 10 + (buffer[i] - '0');
        char expected = CheckMap[number % 31];
        if (check != expected)
        {
            result = new ValidationResult<HetuValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<HetuValue>(true, new HetuValue(new string(buffer)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 11) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int year = rng.Next(1900, 2000);
        int month = rng.Next(1, 13);
        int day = rng.Next(1, DateTime.DaysInMonth(year, month) + 1);
        int serial = rng.Next(0, 1000);
        Span<char> buf = destination[..11];
        int yy = year % 100;
        buf[0] = (char)('0' + day / 10);
        buf[1] = (char)('0' + day % 10);
        buf[2] = (char)('0' + month / 10);
        buf[3] = (char)('0' + month % 10);
        buf[4] = (char)('0' + yy / 10);
        buf[5] = (char)('0' + yy % 10);
        buf[6] = '-';
        buf[7] = (char)('0' + serial / 100);
        buf[8] = (char)('0' + (serial / 10) % 10);
        buf[9] = (char)('0' + serial % 10);
        int number = 0;
        for (int i = 0; i < 6; i++)
            number = number * 10 + (buf[i] - '0');
        for (int i = 7; i < 10; i++)
            number = number * 10 + (buf[i] - '0');
        buf[10] = CheckMap[number % 31];
        written = 11;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = char.ToUpperInvariant(ch);
        }
        return true;
    }
}

