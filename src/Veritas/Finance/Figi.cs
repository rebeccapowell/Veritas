using System;
using Veritas.Algorithms;

namespace Veritas.Finance;

/// <summary>Represents a validated Financial Instrument Global Identifier (FIGI).</summary>
public readonly struct FigiValue
{
    /// <summary>Gets the normalized FIGI.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="FigiValue"/> struct.</summary>
    /// <param name="value">Normalized FIGI.</param>
    public FigiValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Financial Instrument Global Identifiers (FIGI).</summary>
public static class Figi
{
    /// <summary>Attempts to validate the supplied FIGI.</summary>
    /// <param name="input">Candidate FIGI.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<FigiValue> result)
    {
        Span<char> buf = stackalloc char[12];
        if (!Normalize(input, buf, out int len) || len != 12)
        {
            result = new ValidationResult<FigiValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> digits = stackalloc char[24];
        int di = 0;
        for (int i = 0; i < 11; i++)
        {
            char ch = buf[i];
            int v = ch <= '9' ? ch - '0' : ch - 'A' + 10;
            if (v >= 10)
            {
                digits[di++] = (char)('0' + v / 10);
                digits[di++] = (char)('0' + v % 10);
            }
            else
            {
                digits[di++] = (char)('0' + v);
            }
        }
        int check = Luhn.ComputeCheckDigit(digits[..di]);
        if (buf[11] - '0' != check)
        {
            result = new ValidationResult<FigiValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<FigiValue>(true, new FigiValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random FIGI into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated FIGI.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random FIGI using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated FIGI.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 12)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        Span<char> buf = destination[..12];
        for (int i = 0; i < 11; i++)
            buf[i] = alphabet[rng.Next(alphabet.Length)];
        Span<char> digits = stackalloc char[24];
        int di = 0;
        for (int i = 0; i < 11; i++)
        {
            char ch = buf[i];
            int v = ch <= '9' ? ch - '0' : ch - 'A' + 10;
            if (v >= 10)
            {
                digits[di++] = (char)('0' + v / 10);
                digits[di++] = (char)('0' + v % 10);
            }
            else
            {
                digits[di++] = (char)('0' + v);
            }
        }
        buf[11] = (char)('0' + Luhn.ComputeCheckDigit(digits[..di]));
        written = 12;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            char u = char.ToUpperInvariant(ch);
            if (!(u >= '0' && u <= '9' || u >= 'A' && u <= 'Z'))
            {
                len = 0;
                return false;
            }
            if (len >= dest.Length)
            {
                len = 0;
                return false;
            }
            dest[len++] = u;
        }
        return true;
    }
}

