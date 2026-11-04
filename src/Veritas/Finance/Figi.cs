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
        if (!HasValidStructure(buf))
        {
            result = new ValidationResult<FigiValue>(false, default, ValidationError.Format);
            return false;
        }
        int check = ComputeCheckDigit(buf[..11]);
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
        const string alphabet = "0123456789BCDFGHJKLMNPQRSTVWXYZ";
        Span<char> buf = destination[..12];
        buf[0] = buf[1] = 'B';
        buf[2] = 'G';
        for (int i = 3; i < 11; i++) buf[i] = alphabet[rng.Next(alphabet.Length)];
        buf[11] = (char)('0' + ComputeCheckDigit(buf[..11]));
        written = 12;
        return true;
    }

    private static bool HasValidStructure(ReadOnlySpan<char> value)
    {
        const string consonants = "BCDFGHJKLMNPQRSTVWXYZ";
        if (consonants.IndexOf(value[0]) < 0 || consonants.IndexOf(value[1]) < 0 || value[2] != 'G' || value[11] < '0' || value[11] > '9') return false;
        if (value[..2] is "BS" or "BM" or "GG" or "GB" or "VG" or "GH" or "KY") return false;
        for (int i = 3; i < 11; i++)
            if (!(value[i] >= '0' && value[i] <= '9') && consonants.IndexOf(value[i]) < 0) return false;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> body)
    {
        int sum = 0;
        for (int i = 0; i < body.Length; i++)
        {
            int value = body[i] <= '9' ? body[i] - '0' : body[i] - 'A' + 10;
            value *= (i % 2 == 0 ? 1 : 2);
            sum += value / 10 + value % 10;
        }
        return (10 - sum % 10) % 10;
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

