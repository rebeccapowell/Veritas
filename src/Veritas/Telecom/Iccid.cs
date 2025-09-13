using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Telecom;

/// <summary>Represents a validated Integrated Circuit Card Identifier (ICCID).</summary>
public readonly struct IccidValue
{
    /// <summary>Gets the normalized ICCID string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="IccidValue"/> struct.</summary>
    /// <param name="value">The code string.</param>
    public IccidValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for ICCIDs.</summary>
public static class Iccid
{
    /// <summary>Attempts to validate the supplied input as an ICCID.</summary>
    /// <param name="input">Candidate code to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IccidValue> result)
    {
        Span<char> digits = stackalloc char[20];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<IccidValue>(false, default, ValidationError.Charset); return false; }
            if (len >= 20) { result = new ValidationResult<IccidValue>(false, default, ValidationError.Length); return false; }
            digits[len++] = ch;
        }
        if (len < 19 || len > 20) { result = new ValidationResult<IccidValue>(false, default, ValidationError.Length); return false; }
        if (!Luhn.Validate(digits[..len])) { result = new ValidationResult<IccidValue>(false, default, ValidationError.Checksum); return false; }
        result = new ValidationResult<IccidValue>(true, new IccidValue(new string(digits[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random ICCID into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random ICCID using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        const int length = 20;
        if (destination.Length < length)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < length - 1; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[length - 1] = (char)('0' + Luhn.ComputeCheckDigit(destination[..(length - 1)]));
        written = length;
        return true;
    }
}

