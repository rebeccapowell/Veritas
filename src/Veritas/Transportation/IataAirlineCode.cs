using System;

namespace Veritas.Transportation;

/// <summary>Represents a validated IATA airline code.</summary>
public readonly struct IataAirlineCodeValue
{
    /// <summary>Gets the normalized IATA airline code.</summary>
    public string Value { get; }
    /// <summary>Initializes a new instance of the <see cref="IataAirlineCodeValue"/> struct.</summary>
    /// <param name="value">Normalized IATA airline code.</param>
    public IataAirlineCodeValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for IATA airline codes.</summary>
public static class IataAirlineCode
{
    /// <summary>Attempts to validate the supplied IATA airline code.</summary>
    /// <param name="input">Candidate code.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IataAirlineCodeValue> result)
    {
        Span<char> buf = stackalloc char[2];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (len == 2)
            {
                result = new ValidationResult<IataAirlineCodeValue>(false, default, ValidationError.Length);
                return false;
            }
            char u = char.ToUpperInvariant(ch);
            if (!(u >= 'A' && u <= 'Z') && !(u >= '0' && u <= '9'))
            {
                result = new ValidationResult<IataAirlineCodeValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[len++] = u;
        }
        if (len != 2)
        {
            result = new ValidationResult<IataAirlineCodeValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<IataAirlineCodeValue>(true, new IataAirlineCodeValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random IATA airline code into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random IATA airline code using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 2)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 2; i++)
        {
            int v = rng.Next(36);
            destination[i] = v < 26 ? (char)('A' + v) : (char)('0' + (v - 26));
        }
        written = 2;
        return true;
    }
}

