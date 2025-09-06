using System;
using Veritas;

namespace Veritas.Finance;

public readonly struct CusipValue
{
    public string Value { get; }
    public CusipValue(string value) => Value = value;
}

public static class Cusip
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CusipValue> result)
    {
        Span<char> chars = stackalloc char[9];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<CusipValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 9)
        {
            result = new ValidationResult<CusipValue>(false, default, ValidationError.Length);
            return true;
        }
        int check = ComputeCheckDigit(chars[..8]);
        if (chars[8] - '0' != check)
        {
            result = new ValidationResult<CusipValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<CusipValue>(true, new CusipValue(new string(chars)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 9) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ*@#";
        Span<char> chars = destination[..9];
        for (int i = 0; i < 8; i++)
            chars[i] = alphabet[rng.Next(alphabet.Length)];
        chars[8] = (char)('0' + ComputeCheckDigit(chars[..8]));
        written = 9;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> chars)
    {
        int sum = 0;
        bool dbl = true;
        for (int i = chars.Length - 1; i >= 0; i--)
        {
            int v = ValueOf(chars[i]);
            if (dbl)
            {
                v *= 2;
                if (v > 9) v -= 9;
            }
            sum += v;
            dbl = !dbl;
        }
        return (10 - (sum % 10)) % 10;
    }

    private static int ValueOf(char c)
    {
        if (char.IsDigit(c)) return c - '0';
        if (c >= 'A' && c <= 'Z') return c - 'A' + 10;
        return c switch
        {
            '*' => 36,
            '@' => 37,
            '#' => 38,
            _ => -1
        };
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            char c = char.ToUpperInvariant(ch);
            if (!(char.IsLetterOrDigit(c) || c == '*' || c == '@' || c == '#')) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = c;
        }
        return true;
    }
}
