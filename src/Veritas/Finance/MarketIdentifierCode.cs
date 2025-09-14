using System;

namespace Veritas.Finance;

/// <summary>Represents a validated Market Identifier Code (MIC).</summary>
public readonly struct MarketIdentifierCodeValue
{
    /// <summary>Gets the normalized MIC.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="MarketIdentifierCodeValue"/> struct.</summary>
    /// <param name="value">Normalized MIC.</param>
    public MarketIdentifierCodeValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Market Identifier Codes.</summary>
public static class MarketIdentifierCode
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>Attempts to validate the supplied MIC.</summary>
    /// <param name="input">Candidate MIC.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MarketIdentifierCodeValue> result)
    {
        Span<char> buf = stackalloc char[4];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch >= 'A' && ch <= 'Z')
            {
                if (len == 4) { result = new ValidationResult<MarketIdentifierCodeValue>(false, default, ValidationError.Length); return false; }
                buf[len++] = ch;
            }
            else if (ch >= 'a' && ch <= 'z')
            {
                if (len == 4) { result = new ValidationResult<MarketIdentifierCodeValue>(false, default, ValidationError.Length); return false; }
                buf[len++] = char.ToUpperInvariant(ch);
            }
            else
            {
                result = new ValidationResult<MarketIdentifierCodeValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        if (len != 4)
        {
            result = new ValidationResult<MarketIdentifierCodeValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<MarketIdentifierCodeValue>(true, new MarketIdentifierCodeValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random MIC into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated MIC.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random MIC using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated MIC.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 4)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 4; i++)
            destination[i] = Alphabet[rng.Next(Alphabet.Length)];
        written = 4;
        return true;
    }
}

