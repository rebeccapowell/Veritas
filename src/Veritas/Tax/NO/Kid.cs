using System;
using Veritas;

namespace Veritas.Tax.NO;

public readonly struct KidValue
{
    public string Value { get; }
    public KidValue(string value) => Value = value;
}

public enum KidVariant
{
    Mod10,
    Mod11
}

/// <summary>
/// Norway KID (OCR reference) - variable length with mod-10 or mod-11 checksum.
/// </summary>
public static class Kid
{
    public static bool TryValidate(ReadOnlySpan<char> input, KidVariant variant, out ValidationResult<KidValue> result)
    {
        Span<char> digits = stackalloc char[25];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<KidValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len < 2 || len > 25)
        {
            result = new ValidationResult<KidValue>(false, default, ValidationError.Length);
            return true;
        }
        bool ok = variant == KidVariant.Mod10 ? VerifyMod10(digits[..len]) : VerifyMod11(digits[..len]);
        if (!ok)
        {
            result = new ValidationResult<KidValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<KidValue>(true, new KidValue(new string(digits[..len])), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(KidVariant variant, int length, Span<char> destination, out int written)
        => TryGenerate(variant, default, length, destination, out written);

    public static bool TryGenerate(KidVariant variant, in GenerationOptions options, int length, Span<char> destination, out int written)
    {
        written = 0;
        if (length < 2 || length > 25 || destination.Length < length) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < length - 1; i++)
            destination[i] = (char)('0' + rng.Next(0, 10));
        if (variant == KidVariant.Mod10)
            destination[length - 1] = (char)('0' + ComputeMod10(destination[..(length-1)]));
        else
            destination[length - 1] = (char)('0' + ComputeMod11(destination[..(length-1)]));
        written = length;
        return true;
    }

    private static bool VerifyMod10(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        bool dbl = true;
        for (int i = digits.Length - 2; i >= 0; i--)
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
        int check = (10 - (sum % 10)) % 10;
        return check == digits[^1] - '0';
    }

    private static int ComputeMod10(ReadOnlySpan<char> digits)
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
        return (10 - (sum % 10)) % 10;
    }

    private static bool VerifyMod11(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        int weight = 2;
        for (int i = digits.Length - 2; i >= 0; i--)
        {
            sum += (digits[i] - '0') * weight;
            weight = weight == 7 ? 2 : weight + 1;
        }
        int check = 11 - sum % 11;
        if (check == 11) check = 0;
        if (check == 10) return false;
        return check == digits[^1] - '0';
    }

    private static int ComputeMod11(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        int weight = 2;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            sum += (digits[i] - '0') * weight;
            weight = weight == 7 ? 2 : weight + 1;
        }
        int check = 11 - sum % 11;
        if (check == 11) check = 0;
        if (check == 10) check = 0; // rare; set to 0
        return check;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch==' '||ch=='-') continue;
            if (!char.IsDigit(ch) || len >= dest.Length)
            {
                len = 0; return false;
            }
            dest[len++] = ch;
        }
        return true;
    }
}

