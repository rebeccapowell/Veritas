using System;
using Veritas;

namespace Veritas.Tax.IE;

public readonly struct PpsnValue
{
    public string Value { get; }
    public PpsnValue(string value) => Value = value;
}

/// <summary>
/// Ireland PPSN (Personal Public Service Number) (7 digits plus 1–2 letters, mod-23 checksum).
/// </summary>
public static class Ppsn
{
    private const string CheckMap = "WABCDEFGHIJKLMNOPQRSTUV";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PpsnValue> result)
    {
        Span<char> buffer = stackalloc char[9];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<PpsnValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 8 && len != 9)
        {
            result = new ValidationResult<PpsnValue>(false, default, ValidationError.Length);
            return true;
        }
        for (int i = 0; i < 7; i++)
            if (!char.IsDigit(buffer[i]))
            {
                result = new ValidationResult<PpsnValue>(false, default, ValidationError.Format);
                return true;
            }
        char check = buffer[7];
        if (!char.IsLetter(check))
        {
            result = new ValidationResult<PpsnValue>(false, default, ValidationError.Format);
            return true;
        }
        char second = len == 9 ? buffer[8] : 'W';
        if (len == 9 && !char.IsLetter(second))
        {
            result = new ValidationResult<PpsnValue>(false, default, ValidationError.Format);
            return true;
        }
        int sum = 0;
        int[] weights = { 8, 7, 6, 5, 4, 3, 2 };
        for (int i = 0; i < 7; i++)
            sum += (buffer[i] - '0') * weights[i];
        int secondVal = LetterValue(second);
        if (secondVal < 0)
        {
            result = new ValidationResult<PpsnValue>(false, default, ValidationError.Format);
            return true;
        }
        sum += secondVal * 9;
        char expected = CheckMap[sum % 23];
        if (check != expected)
        {
            result = new ValidationResult<PpsnValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<PpsnValue>(true, new PpsnValue(new string(buffer[..len])), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 9) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> buf = destination[..9];
        for (int i = 0; i < 7; i++)
            buf[i] = (char)('0' + rng.Next(10));
        buf[8] = (char)('A' + rng.Next(26));
        int sum = 0;
        int[] weights = { 8, 7, 6, 5, 4, 3, 2 };
        for (int i = 0; i < 7; i++)
            sum += (buf[i] - '0') * weights[i];
        sum += LetterValue(buf[8]) * 9;
        buf[7] = CheckMap[sum % 23];
        written = 9;
        return true;
    }

    private static int LetterValue(char c)
    {
        if (c == 'W') return 0;
        if (c < 'A' || c > 'Z') return -1;
        return c - 'A' + 1;
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

