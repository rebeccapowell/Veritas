using System;

namespace Veritas.Crypto;

/// <summary>Represents a validated Ethereum transaction hash.</summary>
public readonly struct EthereumTransactionHashValue
{
    /// <summary>Gets the normalized transaction hash.</summary>
    public string Value { get; }
    /// <summary>Initializes a new instance of the <see cref="EthereumTransactionHashValue"/> struct.</summary>
    /// <param name="value">Normalized transaction hash.</param>
    public EthereumTransactionHashValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Ethereum transaction hashes.</summary>
public static class EthereumTransactionHash
{
    /// <summary>Attempts to validate the supplied transaction hash.</summary>
    /// <param name="input">Candidate transaction hash.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EthereumTransactionHashValue> result)
    {
        if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            input = input[2..];
        if (input.Length != 64)
        {
            result = new ValidationResult<EthereumTransactionHashValue>(false, default, ValidationError.Length);
            return false;
        }
        for (int i = 0; i < input.Length; i++)
        {
            if (!Uri.IsHexDigit(input[i]))
            {
                result = new ValidationResult<EthereumTransactionHashValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        string value = "0x" + new string(input).ToLowerInvariant();
        result = new ValidationResult<EthereumTransactionHashValue>(true, new EthereumTransactionHashValue(value), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Ethereum transaction hash into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated hash.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Ethereum transaction hash using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated hash.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 66)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<byte> bytes = stackalloc byte[32];
        rng.NextBytes(bytes);
        var hex = Convert.ToHexString(bytes).ToLowerInvariant();
        destination[0] = '0';
        destination[1] = 'x';
        hex.AsSpan().CopyTo(destination[2..]);
        written = 66;
        return true;
    }
}
