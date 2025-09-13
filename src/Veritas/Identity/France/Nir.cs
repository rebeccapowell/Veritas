using System;
using Veritas;

namespace Veritas.Identity.France;

/// <summary>Provides validation and generation for French NIR (INSEE) numbers.</summary>
public static class Nir
{
    /// <summary>Attempts to validate the supplied input as a NIR number.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NirValue> result)
    {
        Span<char> digits = stackalloc char[15];
        if (!Normalize(input, digits, out int len) || len != 15)
        {
            result = new ValidationResult<NirValue>(false, default, ValidationError.Length);
            return false;
        }
        int mod = 0;
        for (int i = 0; i < 13; i++)
        {
            char ch = digits[i];
            int d;
            if (ch == 'A') d = 0;
            else if (ch == 'B') d = 1;
            else { d = ch - '0'; if ((uint)d > 9) { result = new ValidationResult<NirValue>(false, default, ValidationError.Charset); return false; } }
            mod = (mod * 10 + d) % 97;
        }
        int key = 97 - mod;
        int k1 = digits[13] - '0';
        int k2 = digits[14] - '0';
        if ((uint)k1 > 9 || (uint)k2 > 9)
        {
            result = new ValidationResult<NirValue>(false, default, ValidationError.Charset);
            return false;
        }
        int expected = k1 * 10 + k2;
        if (expected != key)
        {
            result = new ValidationResult<NirValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<NirValue>(true, new NirValue(new string(digits)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a synthetic NIR number.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a synthetic NIR number.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 15)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 13; i++)
            destination[i] = (char)('0' + rng.Next(10));
        if (rng.Next(10) == 0)
            destination[6] = rng.Next(2) == 0 ? 'A' : 'B';
        int mod = 0;
        for (int i = 0; i < 13; i++)
        {
            char ch = destination[i];
            int d = ch == 'A' ? 0 : ch == 'B' ? 1 : ch - '0';
            mod = (mod * 10 + d) % 97;
        }
        int key = 97 - mod;
        destination[13] = (char)('0' + key / 10);
        destination[14] = (char)('0' + key % 10);
        written = 15;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-' || ch == '.')
                continue;
            char up = char.ToUpperInvariant(ch);
            if ((uint)(up - '0') <= 9 || up == 'A' || up == 'B')
            {
                if (len >= dest.Length) { len = 0; return false; }
                dest[len++] = up;
            }
            else
            {
                len = 0;
                return false;
            }
        }
        return true;
    }
}
