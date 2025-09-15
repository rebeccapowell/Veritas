using System;

namespace Veritas.Geospatial;

/// <summary>Represents a validated geohash.</summary>
public readonly struct GeohashValue
{
    /// <summary>Gets the normalized geohash string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="GeohashValue"/> struct.</summary>
    /// <param name="value">Normalized geohash.</param>
    public GeohashValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for geohashes.</summary>
public static class Geohash
{
    private const string Alphabet = "0123456789bcdefghjkmnpqrstuvwxyz";

    /// <summary>Attempts to validate the supplied input as a geohash.</summary>
    /// <param name="input">Candidate geohash to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<GeohashValue> result)
    {
        Span<char> buf = stackalloc char[Math.Min(12, input.Length)];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (len >= 12)
            {
                result = new ValidationResult<GeohashValue>(false, default, ValidationError.Length);
                return false;
            }
            char lower = char.ToLowerInvariant(ch);
            if (Alphabet.IndexOf(lower) < 0)
            {
                result = new ValidationResult<GeohashValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[len++] = lower;
        }
        if (len == 0)
        {
            result = new ValidationResult<GeohashValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<GeohashValue>(true, new GeohashValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random geohash into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated geohash.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random geohash using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated geohash.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        const int DefaultLength = 8;
        if (destination.Length < DefaultLength)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < DefaultLength; i++)
            destination[i] = Alphabet[rng.Next(Alphabet.Length)];
        written = DefaultLength;
        return true;
    }
}

