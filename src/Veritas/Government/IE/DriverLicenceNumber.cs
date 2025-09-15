using System;

namespace Veritas.Government.IE;

/// <summary>Represents a validated Irish driver licence number.</summary>
public readonly struct DriverLicenceNumberValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="DriverLicenceNumberValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public DriverLicenceNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Irish driver licence numbers.</summary>
public static class DriverLicenceNumber
{
    /// <summary>Attempts to validate the supplied Irish driver licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<DriverLicenceNumberValue> result)
    {
        if (input.Length != 9)
        {
            result = new ValidationResult<DriverLicenceNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> buf = stackalloc char[9];
        for (int i = 0; i < 9; i++)
        {
            char ch = input[i];
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<DriverLicenceNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[i] = ch;
        }
        result = new ValidationResult<DriverLicenceNumberValue>(true, new DriverLicenceNumberValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Irish driver licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Irish driver licence number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 9)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 9; i++)
            destination[i] = (char)('0' + rng.Next(10));
        written = 9;
        return true;
    }
}

