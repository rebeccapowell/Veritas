using System;

namespace Veritas.Government.NZ;

/// <summary>Represents a validated New Zealand driver licence number.</summary>
public readonly struct DriverLicenceNumberValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="DriverLicenceNumberValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public DriverLicenceNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for New Zealand driver licence numbers.</summary>
public static class DriverLicenceNumber
{
    /// <summary>Attempts to validate the supplied New Zealand driver licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<DriverLicenceNumberValue> result)
    {
        if (input.Length != 8)
        {
            result = new ValidationResult<DriverLicenceNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> buf = stackalloc char[8];
        for (int i = 0; i < 8; i++)
        {
            char ch = input[i];
            if (ch >= 'a' && ch <= 'z') ch = (char)(ch - 32);
            if (!(ch >= 'A' && ch <= 'Z') && (ch < '0' || ch > '9'))
            {
                result = new ValidationResult<DriverLicenceNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[i] = ch;
        }
        result = new ValidationResult<DriverLicenceNumberValue>(true, new DriverLicenceNumberValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random New Zealand driver licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random New Zealand driver licence number using the supplied options.</summary>
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
        {
            int v = rng.Next(36);
            destination[i] = v < 10 ? (char)('0' + v) : (char)('A' + v - 10);
        }
        written = 8;
        return true;
    }
}

