using System;
using Veritas;

namespace Veritas.Energy.GB;

/// <summary>Represents a validated UK Meter Point Administration Number (MPAN).</summary>
public readonly struct MpanValue
{
    /// <summary>Gets the normalized MPAN string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="MpanValue"/> struct.</summary>
    /// <param name="value">The identifier string.</param>
    public MpanValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for MPAN identifiers.</summary>
public static class Mpan
{
    private static readonly int[] Weights = new[] { 3, 7, 1, 3, 7, 1, 3, 7, 1, 3, 7, 1 };

    /// <summary>Attempts to validate the supplied input as an MPAN.</summary>
    /// <param name="input">Candidate identifier to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MpanValue> result)
    {
        Span<char> digits = stackalloc char[13];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<MpanValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 13)
        {
            result = new ValidationResult<MpanValue>(false, default, ValidationError.Length);
            return false;
        }
        int sum = 0;
        for (int i = 0; i < 12; i++)
            sum += (digits[i] - '0') * Weights[i];
        int r = sum % 11;
        int check = r == 10 ? 0 : r;
        if (digits[12] - '0' != check)
        {
            result = new ValidationResult<MpanValue>(false, default, ValidationError.Checksum);
            return false;
        }
        string value = new string(digits);
        result = new ValidationResult<MpanValue>(true, new MpanValue(value), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random MPAN into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random MPAN using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 13)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 12; i++)
            destination[i] = (char)('0' + rng.Next(10));
        int sum = 0;
        for (int i = 0; i < 12; i++)
            sum += (destination[i] - '0') * Weights[i];
        int r = sum % 11;
        destination[12] = (char)('0' + (r == 10 ? 0 : r));
        written = 13;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}

