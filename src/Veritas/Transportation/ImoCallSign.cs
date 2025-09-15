using System;

namespace Veritas.Transportation;

/// <summary>Represents a validated IMO call sign.</summary>
public readonly struct ImoCallSignValue
{
    /// <summary>Gets the normalized call sign.</summary>
    public string Value { get; }
    /// <summary>Initializes a new instance of the <see cref="ImoCallSignValue"/> struct.</summary>
    /// <param name="value">Normalized call sign.</param>
    public ImoCallSignValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for IMO call signs.</summary>
public static class ImoCallSign
{
    /// <summary>Attempts to validate the supplied IMO call sign.</summary>
    /// <param name="input">Candidate call sign.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ImoCallSignValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (len == input.Length)
            {
                result = new ValidationResult<ImoCallSignValue>(false, default, ValidationError.Length);
                return false;
            }
            char u = char.ToUpperInvariant(ch);
            if (!((u >= 'A' && u <= 'Z') || (u >= '0' && u <= '9')))
            {
                result = new ValidationResult<ImoCallSignValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[len++] = u;
        }
        if (len < 3 || len > 7)
        {
            result = new ValidationResult<ImoCallSignValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<ImoCallSignValue>(true, new ImoCallSignValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random IMO call sign into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated call sign.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random IMO call sign using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated call sign.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int len = rng.Next(3, 8);
        if (destination.Length < len)
        {
            written = 0;
            return false;
        }
        for (int i = 0; i < len; i++)
        {
            int v = rng.Next(36);
            destination[i] = v < 26 ? (char)('A' + v) : (char)('0' + (v - 26));
        }
        written = len;
        return true;
    }
}

