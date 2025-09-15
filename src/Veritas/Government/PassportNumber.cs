using System;

namespace Veritas.Government;

/// <summary>Represents a validated passport number.</summary>
public readonly struct PassportNumberValue
{
    /// <summary>Gets the normalized passport number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="PassportNumberValue"/> struct.</summary>
    /// <param name="value">Normalized passport number.</param>
    public PassportNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for passport numbers.</summary>
public static class PassportNumber
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>Attempts to validate the supplied passport number.</summary>
    /// <param name="input">Candidate passport number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PassportNumberValue> result)
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
                result = new ValidationResult<PassportNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
            if (len > 9)
            {
                result = new ValidationResult<PassportNumberValue>(false, default, ValidationError.Length);
                return false;
            }
        }
        if (len < 6)
        {
            result = new ValidationResult<PassportNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<PassportNumberValue>(true, new PassportNumberValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random passport number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated passport number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random passport number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated passport number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int len = rng.Next(6, 10); // 6..9
        if (destination.Length < len)
        {
            written = 0;
            return false;
        }
        for (int i = 0; i < len; i++)
            destination[i] = Alphabet[rng.Next(Alphabet.Length)];
        written = len;
        return true;
    }
}

