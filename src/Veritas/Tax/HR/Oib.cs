using System;
using Veritas;

namespace Veritas.Tax.HR;

public readonly struct OibValue
{
    public string Value { get; }
    public OibValue(string value) => Value = value;
}

/// <summary>
/// Croatia OIB (11 digits, ISO 7064 mod 11,10 checksum).
/// </summary>
public static class Oib
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<OibValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<OibValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 11)
        {
            result = new ValidationResult<OibValue>(false, default, ValidationError.Length);
            return false;
        }
        int check = digits[10] - '0';
        if (ComputeCheck(digits[..10]) != check)
        {
            result = new ValidationResult<OibValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<OibValue>(true, new OibValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 11) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 10; i++)
            destination[i] = (char)('0' + rng.Next(0, 10));
        int check = ComputeCheck(destination[..10]);
        destination[10] = (char)('0' + check);
        written = 11;
        return true;
    }

    private static int ComputeCheck(ReadOnlySpan<char> digits)
    {
        int acc = 10;
        foreach (var ch in digits)
        {
            int sum = (acc + (ch - '0')) % 10;
            if (sum == 0) sum = 10;
            acc = (sum * 2) % 11;
        }
        int check = 11 - acc;
        if (check == 10) check = 0;
        else if (check == 11) check = 0;
        return check;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
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

