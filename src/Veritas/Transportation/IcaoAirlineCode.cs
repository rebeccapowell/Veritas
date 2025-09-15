using System;

namespace Veritas.Transportation;

/// <summary>Represents a validated ICAO airline code.</summary>
public readonly struct IcaoAirlineCodeValue
{
    /// <summary>Gets the normalized ICAO airline code.</summary>
    public string Value { get; }
    /// <summary>Initializes a new instance of the <see cref="IcaoAirlineCodeValue"/> struct.</summary>
    /// <param name="value">Normalized ICAO airline code.</param>
    public IcaoAirlineCodeValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for ICAO airline codes.</summary>
public static class IcaoAirlineCode
{
    /// <summary>Attempts to validate the supplied ICAO airline code.</summary>
    /// <param name="input">Candidate code.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IcaoAirlineCodeValue> result)
    {
        Span<char> buf = stackalloc char[3];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (len == 3)
            {
                result = new ValidationResult<IcaoAirlineCodeValue>(false, default, ValidationError.Length);
                return false;
            }
            char u = char.ToUpperInvariant(ch);
            if (u < 'A' || u > 'Z')
            {
                result = new ValidationResult<IcaoAirlineCodeValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[len++] = u;
        }
        if (len != 3)
        {
            result = new ValidationResult<IcaoAirlineCodeValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<IcaoAirlineCodeValue>(true, new IcaoAirlineCodeValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random ICAO airline code into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random ICAO airline code using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 3)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 3; i++)
            destination[i] = (char)('A' + rng.Next(26));
        written = 3;
        return true;
    }
}

