using System;
using Veritas;

namespace Veritas.Telecom;

public readonly struct MeidValue { public string Value { get; } public MeidValue(string v) => Value = v; }

public static class Meid
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MeidValue> result)
    {
        Span<char> buf = stackalloc char[18];
        if (!Normalize(input, buf, out int len))
        {
            result = new ValidationResult<MeidValue>(false, default, ValidationError.Charset);
            return true;
        }
        if (len == 14)
        {
            for (int i = 0; i < 14; i++)
            {
                char c = buf[i];
                if (!Uri.IsHexDigit(c))
                {
                    result = new ValidationResult<MeidValue>(false, default, ValidationError.Charset);
                    return true;
                }
                buf[i] = char.ToUpperInvariant(c);
            }
            result = new ValidationResult<MeidValue>(true, new MeidValue(new string(buf[..14])), ValidationError.None);
            return true;
        }
        else if (len == 18)
        {
            for (int i = 0; i < 18; i++)
            {
                if (!char.IsDigit(buf[i]))
                {
                    result = new ValidationResult<MeidValue>(false, default, ValidationError.Charset);
                    return true;
                }
            }
            int check = Luhn(buf[..17]);
            if (buf[17] != (char)('0' + check))
            {
                result = new ValidationResult<MeidValue>(false, default, ValidationError.Checksum);
                return true;
            }
            result = new ValidationResult<MeidValue>(true, new MeidValue(new string(buf[..18])), ValidationError.None);
            return true;
        }
        result = new ValidationResult<MeidValue>(false, default, ValidationError.Length);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 14) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 14; i++)
        {
            int v = rng.Next(16);
            destination[i] = (char)(v < 10 ? '0' + v : 'A' + v - 10);
        }
        written = 14;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-' || ch == '.') continue;
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }

    private static int Luhn(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        bool dbl = true;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i] - '0';
            if (dbl)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            dbl = !dbl;
        }
        return (10 - sum % 10) % 10;
    }
}
