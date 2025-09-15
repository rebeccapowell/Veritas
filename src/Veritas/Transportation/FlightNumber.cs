using System;

namespace Veritas.Transportation;

/// <summary>Represents a validated flight number.</summary>
public readonly struct FlightNumberValue
{
    /// <summary>Gets the normalized flight number.</summary>
    public string Value { get; }
    /// <summary>Initializes a new instance of the <see cref="FlightNumberValue"/> struct.</summary>
    /// <param name="value">Normalized flight number.</param>
    public FlightNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for flight numbers.</summary>
public static class FlightNumber
{
    /// <summary>Attempts to validate the supplied flight number.</summary>
    /// <param name="input">Candidate flight number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<FlightNumberValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (len == input.Length)
            {
                result = new ValidationResult<FlightNumberValue>(false, default, ValidationError.Length);
                return false;
            }
            buf[len++] = char.ToUpperInvariant(ch);
        }
        if (len < 3 || len > 6)
        {
            result = new ValidationResult<FlightNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        for (int i = 0; i < 2; i++)
        {
            char c = buf[i];
            if (!((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')))
            {
                result = new ValidationResult<FlightNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        for (int i = 2; i < len; i++)
        {
            char c = buf[i];
            if (c < '0' || c > '9')
            {
                result = new ValidationResult<FlightNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        result = new ValidationResult<FlightNumberValue>(true, new FlightNumberValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random flight number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated flight number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random flight number using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated flight number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int digits = rng.Next(1, 5);
        int total = 2 + digits;
        if (destination.Length < total)
        {
            written = 0;
            return false;
        }
        for (int i = 0; i < 2; i++)
            destination[i] = (char)('A' + rng.Next(26));
        for (int i = 0; i < digits; i++)
            destination[2 + i] = (char)('0' + rng.Next(10));
        written = total;
        return true;
    }
}

