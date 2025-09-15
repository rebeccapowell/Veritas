using System;
using Veritas.Algorithms;

namespace Veritas.Crypto;

/// <summary>Represents a validated Bitcoin address.</summary>
public readonly struct BitcoinAddressValue
{
    /// <summary>Gets the normalized Bitcoin address.</summary>
    public string Value { get; }
    /// <summary>Initializes a new instance of the <see cref="BitcoinAddressValue"/> struct.</summary>
    /// <param name="value">Normalized Bitcoin address.</param>
    public BitcoinAddressValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Bitcoin addresses.</summary>
public static class BitcoinAddress
{
    /// <summary>Attempts to validate the supplied Bitcoin address.</summary>
    /// <param name="input">Candidate Bitcoin address.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<BitcoinAddressValue> result)
    {
        Span<byte> payload = stackalloc byte[25];
        if (Base58Check.TryDecode(input, payload, out var len) && len == 21)
        {
            byte version = payload[0];
            if (version == 0x00 || version == 0x05)
            {
                result = new ValidationResult<BitcoinAddressValue>(true, new BitcoinAddressValue(new string(input)), ValidationError.None);
                return true;
            }
            result = new ValidationResult<BitcoinAddressValue>(false, default, ValidationError.Format);
            return false;
        }
        result = new ValidationResult<BitcoinAddressValue>(false, default, ValidationError.Format);
        return false;
    }

    /// <summary>Attempts to generate a random Bitcoin address into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated address.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Bitcoin address using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated address.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        Span<byte> data = stackalloc byte[21];
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        data[0] = rng.Next(2) == 0 ? (byte)0x00 : (byte)0x05;
        rng.NextBytes(data[1..]);
        return Base58Check.TryEncode(data, destination, out written);
    }
}
