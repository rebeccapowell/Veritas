using System;
using System.Globalization;
using Veritas;

namespace Veritas.Telecom;

/// <summary>Represents a validated Autonomous System Number (ASN).</summary>
public readonly struct AsnValue
{
    /// <summary>Gets the numeric ASN value.</summary>
    public uint Value { get; }

    /// <summary>Initializes a new instance of the <see cref="AsnValue"/> struct.</summary>
    /// <param name="value">The parsed ASN.</param>
    public AsnValue(uint value) => Value = value;
}

/// <summary>Provides validation and generation for Autonomous System Numbers.</summary>
public static class Asn
{
    /// <summary>Attempts to validate the supplied input as an ASN.</summary>
    /// <param name="input">Candidate ASN to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation executed; the <see cref="ValidationResult{T}.IsValid"/> property indicates success.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<AsnValue> result)
    {
        if (uint.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
        {
            result = new ValidationResult<AsnValue>(true, new AsnValue(value), ValidationError.None);
            return true;
        }
        result = new ValidationResult<AsnValue>(false, default, ValidationError.Format);
        return true;
    }

    /// <summary>Attempts to generate a random ASN into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random ASN using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        uint value = (uint)rng.NextInt64(0, (long)uint.MaxValue + 1);
        var s = value.ToString(CultureInfo.InvariantCulture);
        if (destination.Length < s.Length) { written = 0; return false; }
        s.AsSpan().CopyTo(destination);
        written = s.Length;
        return true;
    }
}

