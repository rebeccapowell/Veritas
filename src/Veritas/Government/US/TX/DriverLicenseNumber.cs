using System;

namespace Veritas.Government.US.TX;

/// <summary>Represents a validated Texas driver license number.</summary>
public readonly struct DriverLicenseNumberValue
{
    /// <summary>Gets the normalized driver license number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="DriverLicenseNumberValue"/> struct.</summary>
    /// <param name="value">Normalized driver license number.</param>
    public DriverLicenseNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Texas driver license numbers.</summary>
public static class DriverLicenseNumber
{
    /// <summary>Attempts to validate the supplied Texas driver license number.</summary>
    /// <param name="input">Candidate license number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<DriverLicenseNumberValue> result)
    {
        Span<char> buf = stackalloc char[8];
        if (input.Length != 8)
        {
            result = new ValidationResult<DriverLicenseNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        for (int i = 0; i < 8; i++)
        {
            char ch = input[i];
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<DriverLicenseNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[i] = ch;
        }
        result = new ValidationResult<DriverLicenseNumberValue>(true, new DriverLicenseNumberValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Texas driver license number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Texas driver license number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 8)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 8; i++)
            destination[i] = (char)('0' + rng.Next(10));
        written = 8;
        return true;
    }
}

