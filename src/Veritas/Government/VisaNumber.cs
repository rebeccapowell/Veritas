using System;

namespace Veritas.Government;

/// <summary>Represents a validated visa number.</summary>
public readonly struct VisaNumberValue
{
    /// <summary>Gets the normalized visa number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="VisaNumberValue"/> struct.</summary>
    /// <param name="value">Normalized visa number.</param>
    public VisaNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for visa numbers.</summary>
public static class VisaNumber
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>Attempts to validate the supplied visa number.</summary>
    /// <param name="input">Candidate visa number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<VisaNumberValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (ch >= '0' && ch <= '9')
            {
                buf[len++] = ch;
            }
            else if ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'))
            {
                buf[len++] = char.ToUpperInvariant(ch);
            }
            else
            {
                result = new ValidationResult<VisaNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
            if (len > 8)
            {
                result = new ValidationResult<VisaNumberValue>(false, default, ValidationError.Length);
                return false;
            }
        }
        if (len != 8)
        {
            result = new ValidationResult<VisaNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<VisaNumberValue>(true, new VisaNumberValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random visa number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated visa number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random visa number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated visa number.</param>
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
            destination[i] = Alphabet[rng.Next(Alphabet.Length)];
        written = 8;
        return true;
    }
}

