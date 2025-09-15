using System;
using Veritas.Algorithms;

namespace Veritas.Government.UK;

/// <summary>Represents a validated UK driving licence number.</summary>
public readonly struct DrivingLicenceNumberValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="DrivingLicenceNumberValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public DrivingLicenceNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for UK driving licence numbers.</summary>
public static class DrivingLicenceNumber
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>Attempts to validate the supplied UK driving licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<DrivingLicenceNumberValue> result)
    {
        Span<char> buf = stackalloc char[16];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ')
                continue;
            if (ch >= '0' && ch <= '9')
            {
                if (len == 16) { result = new ValidationResult<DrivingLicenceNumberValue>(false, default, ValidationError.Length); return false; }
                buf[len++] = ch;
            }
            else if ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'))
            {
                if (len == 16) { result = new ValidationResult<DrivingLicenceNumberValue>(false, default, ValidationError.Length); return false; }
                buf[len++] = char.ToUpperInvariant(ch);
            }
            else
            {
                result = new ValidationResult<DrivingLicenceNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        if (len != 16)
        {
            result = new ValidationResult<DrivingLicenceNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        if (char.ToUpperInvariant(Luhn.ComputeCheckCharacterBase36(buf[..15])) != buf[15])
        {
            result = new ValidationResult<DrivingLicenceNumberValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<DrivingLicenceNumberValue>(true, new DrivingLicenceNumberValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random UK driving licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random UK driving licence number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 16)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 15; i++)
            destination[i] = Alphabet[rng.Next(Alphabet.Length)];
        destination[15] = Luhn.ComputeCheckCharacterBase36(destination[..15]);
        written = 16;
        return true;
    }
}

