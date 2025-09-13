using System;
using Veritas.Algorithms;

namespace Veritas.Tax.HU;

/// <summary>Hungary VAT number (Adószám) identifier (8+1+2 digits, weighted mod-10 checksum).</summary>
public static class Adoszam
{
    /// <summary>Attempts to validate the supplied input as a Hungarian VAT (Adószám) number.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<AdoszamValue> result)
    {
        Span<char> digits = stackalloc char[11];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<AdoszamValue>(false, default, ValidationError.Charset);
                return false;
            }
            if (len >= 11)
            {
                result = new ValidationResult<AdoszamValue>(false, default, ValidationError.Length);
                return false;
            }
            digits[len++] = ch;
        }
        if (len != 11)
        {
            result = new ValidationResult<AdoszamValue>(false, default, ValidationError.Length);
            return false;
        }
        int sum = 0;
        int[] weights = { 9, 7, 3, 1, 9, 7, 3 };
        for (int i = 0; i < 7; i++)
            sum += (digits[i] - '0') * weights[i];
        int check = (10 - (sum % 10)) % 10;
        if (digits[7] - '0' != check)
        {
            result = new ValidationResult<AdoszamValue>(false, default, ValidationError.Checksum);
            return false;
        }
        // basic VAT and county code range checks
        if (digits[8] == '0')
        {
            result = new ValidationResult<AdoszamValue>(false, default, ValidationError.Format);
            return false;
        }
        if (digits[9] == '0' && digits[10] == '0')
        {
            result = new ValidationResult<AdoszamValue>(false, default, ValidationError.Format);
            return false;
        }
        result = new ValidationResult<AdoszamValue>(true, new AdoszamValue(new string(digits)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a synthetic Hungarian VAT (Adószám) number.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a synthetic Hungarian VAT (Adószám) number.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 11)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int[] weights = { 9, 7, 3, 1, 9, 7, 3 };
        for (int i = 0; i < 7; i++)
            destination[i] = (char)('0' + rng.Next(10));
        int sum = 0;
        for (int i = 0; i < 7; i++)
            sum += (destination[i] - '0') * weights[i];
        destination[7] = (char)('0' + (10 - (sum % 10)) % 10);
        destination[8] = (char)('1' + rng.Next(9));
        int county = rng.Next(1, 21);
        destination[9] = (char)('0' + county / 10);
        destination[10] = (char)('0' + county % 10);
        written = 11;
        return true;
    }
}

/// <summary>Represents a validated Hungarian VAT (Adószám) number.</summary>
public readonly struct AdoszamValue
{
    /// <summary>Gets the normalized Adószám string.</summary>
    public string Value { get; }

    /// <summary>Creates a new <see cref="AdoszamValue"/>.</summary>
    public AdoszamValue(string value) => Value = value;

    public override string ToString() => Value;
}
