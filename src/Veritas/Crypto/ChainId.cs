using System;

namespace Veritas.Crypto;

/// <summary>Represents a blockchain chain identifier.</summary>
public readonly struct ChainIdValue
{
    /// <summary>Gets the numeric chain identifier.</summary>
    public uint Value { get; }
    /// <summary>Initializes a new instance of the <see cref="ChainIdValue"/> struct.</summary>
    /// <param name="value">Parsed chain identifier.</param>
    public ChainIdValue(uint value) => Value = value;
}

/// <summary>Provides validation and generation for blockchain chain identifiers.</summary>
public static class ChainId
{
    /// <summary>Attempts to validate the supplied chain identifier.</summary>
    /// <param name="input">Candidate chain identifier.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ChainIdValue> result)
    {
        if (input.IsEmpty)
        {
            result = new ValidationResult<ChainIdValue>(false, default, ValidationError.Length);
            return false;
        }
        uint value = 0;
        foreach (char c in input)
        {
            if (c < '0' || c > '9')
            {
                result = new ValidationResult<ChainIdValue>(false, default, ValidationError.Charset);
                return false;
            }
            value = unchecked(value * 10 + (uint)(c - '0'));
        }
        if (value == 0)
        {
            result = new ValidationResult<ChainIdValue>(false, default, ValidationError.Range);
            return false;
        }
        result = new ValidationResult<ChainIdValue>(true, new ChainIdValue(value), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random chain identifier into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated chain identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random chain identifier using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        uint value = (uint)rng.Next(1, int.MaxValue);
        var s = value.ToString();
        if (destination.Length < s.Length)
        {
            written = 0;
            return false;
        }
        s.AsSpan().CopyTo(destination);
        written = s.Length;
        return true;
    }
}
