using System;

namespace Veritas.Healthcare;

/// <summary>Represents a validated RxNorm identifier.</summary>
public readonly struct RxNormIdentifierValue
{
    /// <summary>Gets the normalized RxNorm identifier.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="RxNormIdentifierValue"/> struct.</summary>
    /// <param name="value">Normalized identifier.</param>
    public RxNormIdentifierValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for RxNorm identifiers.</summary>
public static class RxNormIdentifier
{
    /// <summary>Attempts to validate the supplied RxNorm identifier.</summary>
    /// <param name="input">Candidate identifier.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<RxNormIdentifierValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ')
                continue;
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<RxNormIdentifierValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[len++] = ch;
        }
        if (len == 0 || len > 8)
        {
            result = new ValidationResult<RxNormIdentifierValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<RxNormIdentifierValue>(true, new RxNormIdentifierValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random RxNorm identifier into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random RxNorm identifier using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int digits = rng.Next(1, 9); // up to 8 digits
        if (destination.Length < digits)
        {
            written = 0;
            return false;
        }
        for (int i = 0; i < digits; i++)
            destination[i] = (char)('0' + rng.Next(10));
        written = digits;
        return true;
    }
}

