using System;

namespace Veritas.Geospatial.NL;

/// <summary>Represents a validated Dutch cadastral parcel identifier (Kadastrale Aanduiding).</summary>
public readonly struct KadastraleAanduidingValue
{
    /// <summary>Gets the normalized parcel identifier.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="KadastraleAanduidingValue"/> struct.</summary>
    /// <param name="value">Normalized parcel identifier.</param>
    public KadastraleAanduidingValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Dutch cadastral parcel identifiers.</summary>
public static class KadastraleAanduiding
{
    /// <summary>Attempts to validate the supplied input as a Kadastrale Aanduiding.</summary>
    /// <param name="input">Candidate identifier to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<KadastraleAanduidingValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ')
                continue;
            buf[len++] = char.ToUpperInvariant(ch);
        }
        if (len < 6)
        {
            result = new ValidationResult<KadastraleAanduidingValue>(false, default, ValidationError.Length);
            return false;
        }
        for (int i = 0; i < 4; i++)
        {
            if (i >= len || buf[i] < 'A' || buf[i] > 'Z')
            {
                result = new ValidationResult<KadastraleAanduidingValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        if (buf[4] < 'A' || buf[4] > 'Z')
        {
            result = new ValidationResult<KadastraleAanduidingValue>(false, default, ValidationError.Charset);
            return false;
        }
        if (len == 5)
        {
            result = new ValidationResult<KadastraleAanduidingValue>(false, default, ValidationError.Length);
            return false;
        }
        for (int i = 5; i < len; i++)
        {
            if (buf[i] < '0' || buf[i] > '9')
            {
                result = new ValidationResult<KadastraleAanduidingValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        var normalized = $"{new string(buf[..4])} {buf[4]} {new string(buf[5..len])}";
        result = new ValidationResult<KadastraleAanduidingValue>(true, new KadastraleAanduidingValue(normalized), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Kadastrale Aanduiding into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Kadastrale Aanduiding using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        const int Length = 4 + 1 + 1 + 1 + 4; // municipality + space + section + space + number
        if (destination.Length < Length)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 4; i++)
            destination[i] = (char)('A' + rng.Next(26));
        destination[4] = ' ';
        destination[5] = (char)('A' + rng.Next(26));
        destination[6] = ' ';
        for (int i = 0; i < 4; i++)
            destination[7 + i] = (char)('0' + rng.Next(10));
        written = Length;
        return true;
    }
}

