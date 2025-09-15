using System;
using Veritas.Algorithms;

namespace Veritas.Government.ZA;

/// <summary>Represents a validated South African driver licence number.</summary>
public readonly struct SouthAfricanLicenceNumberValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="SouthAfricanLicenceNumberValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public SouthAfricanLicenceNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for South African driver licence numbers.</summary>
public static class SouthAfricanLicenceNumber
{
    /// <summary>Attempts to validate the supplied South African driver licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<SouthAfricanLicenceNumberValue> result)
    {
        Span<char> digits = stackalloc char[13];
        if (input.Length != 13)
        {
            result = new ValidationResult<SouthAfricanLicenceNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        for (int i = 0; i < 13; i++)
        {
            char ch = input[i];
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<SouthAfricanLicenceNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
            digits[i] = ch;
        }
        if (!Luhn.Validate(digits))
        {
            result = new ValidationResult<SouthAfricanLicenceNumberValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<SouthAfricanLicenceNumberValue>(true, new SouthAfricanLicenceNumberValue(new string(digits)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random South African driver licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random South African driver licence number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 13)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 12; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[12] = (char)('0' + Luhn.ComputeCheckDigit(destination[..12]));
        written = 13;
        return true;
    }
}

