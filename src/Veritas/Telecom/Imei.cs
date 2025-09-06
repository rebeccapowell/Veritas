using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Telecom;

/// <summary>Represents a validated International Mobile Equipment Identity (IMEI).</summary>
public readonly struct ImeiValue
{
    /// <summary>Gets the normalized IMEI string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="ImeiValue"/> struct.</summary>
    /// <param name="value">The code string.</param>
    public ImeiValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for IMEI numbers.</summary>
public static class Imei
{
    /// <summary>Attempts to validate the supplied input as an IMEI.</summary>
    /// <param name="input">Candidate code to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation executed; the <see cref="ValidationResult{T}.IsValid"/> property indicates success.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ImeiValue> result)
    {
        Span<char> digits = stackalloc char[15];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<ImeiValue>(false, default, ValidationError.Charset); return true; }
            if (len >= 15) { result = new ValidationResult<ImeiValue>(false, default, ValidationError.Length); return true; }
            digits[len++] = ch;
        }
        if (len != 15) { result = new ValidationResult<ImeiValue>(false, default, ValidationError.Length); return true; }
        if (!Luhn.Validate(digits)) { result = new ValidationResult<ImeiValue>(false, default, ValidationError.Checksum); return true; }
        result = new ValidationResult<ImeiValue>(true, new ImeiValue(new string(digits)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random IMEI into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random IMEI using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 15)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 14; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[14] = (char)('0' + Luhn.ComputeCheckDigit(destination[..14]));
        written = 15;
        return true;
    }
}

