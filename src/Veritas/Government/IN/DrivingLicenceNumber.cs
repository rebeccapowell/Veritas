using System;

namespace Veritas.Government.IN;

/// <summary>Represents a validated Indian driving licence number.</summary>
public readonly struct DrivingLicenceNumberValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="DrivingLicenceNumberValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public DrivingLicenceNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Indian driving licence numbers.</summary>
public static class DrivingLicenceNumber
{
    /// <summary>Attempts to validate the supplied Indian driving licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<DrivingLicenceNumberValue> result)
    {
        Span<char> buf = stackalloc char[15];
        int idx = 0;
        foreach (char ch in input)
        {
            if (ch == '-' || ch == ' ') continue;
            if (idx >= 15)
            {
                result = new ValidationResult<DrivingLicenceNumberValue>(false, default, ValidationError.Length);
                return false;
            }
            buf[idx++] = ch;
        }
        if (idx != 15)
        {
            result = new ValidationResult<DrivingLicenceNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        if (!(buf[0] >= 'A' && buf[0] <= 'Z') || !(buf[1] >= 'A' && buf[1] <= 'Z'))
        {
            result = new ValidationResult<DrivingLicenceNumberValue>(false, default, ValidationError.Charset);
            return false;
        }
        for (int i = 2; i < 15; i++)
        {
            if (buf[i] < '0' || buf[i] > '9')
            {
                result = new ValidationResult<DrivingLicenceNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        result = new ValidationResult<DrivingLicenceNumberValue>(true, new DrivingLicenceNumberValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Indian driving licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Indian driving licence number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
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
        destination[0] = (char)('A' + rng.Next(26));
        destination[1] = (char)('A' + rng.Next(26));
        for (int i = 2; i < 4; i++)
            destination[i] = (char)('0' + rng.Next(10));
        for (int i = 4; i < 15; i++)
            destination[i] = (char)('0' + rng.Next(10));
        written = 15;
        return true;
    }
}

