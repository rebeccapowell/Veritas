using System;

namespace Veritas.Transportation;

/// <summary>Represents a validated FAA N-number.</summary>
public readonly struct FaaNNumberValue
{
    /// <summary>Gets the normalized FAA N-number.</summary>
    public string Value { get; }
    /// <summary>Initializes a new instance of the <see cref="FaaNNumberValue"/> struct.</summary>
    /// <param name="value">Normalized FAA N-number.</param>
    public FaaNNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for FAA N-numbers.</summary>
public static class FaaNNumber
{
    private const string Letters = "ABCDEFGHJKLMNPQRSTUVWXYZ";

    /// <summary>Attempts to validate the supplied FAA N-number.</summary>
    /// <param name="input">Candidate N-number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<FaaNNumberValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        int digitCount = 0;
        int letterCount = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (len == 0)
            {
                char u = char.ToUpperInvariant(ch);
                if (u != 'N')
                {
                    result = new ValidationResult<FaaNNumberValue>(false, default, ValidationError.Format);
                    return false;
                }
                buf[len++] = 'N';
            }
            else if (letterCount == 0 && ch >= '0' && ch <= '9')
            {
                if (digitCount == 0 && ch == '0')
                {
                    result = new ValidationResult<FaaNNumberValue>(false, default, ValidationError.Range);
                    return false;
                }
                if (++digitCount > 5)
                {
                    result = new ValidationResult<FaaNNumberValue>(false, default, ValidationError.Length);
                    return false;
                }
                buf[len++] = ch;
            }
            else if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z')
            {
                if (digitCount == 0)
                {
                    result = new ValidationResult<FaaNNumberValue>(false, default, ValidationError.Format);
                    return false;
                }
                if (letterCount == 2)
                {
                    result = new ValidationResult<FaaNNumberValue>(false, default, ValidationError.Length);
                    return false;
                }
                char u = char.ToUpperInvariant(ch);
                if (u == 'I' || u == 'O' || Letters.IndexOf(u) < 0)
                {
                    result = new ValidationResult<FaaNNumberValue>(false, default, ValidationError.Charset);
                    return false;
                }
                letterCount++;
                buf[len++] = u;
            }
            else
            {
                result = new ValidationResult<FaaNNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        if (digitCount == 0)
        {
            result = new ValidationResult<FaaNNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        if (digitCount == 5 && letterCount != 0 || digitCount == 4 && letterCount > 1)
        {
            result = new ValidationResult<FaaNNumberValue>(false, default, ValidationError.Format);
            return false;
        }
        result = new ValidationResult<FaaNNumberValue>(true, new FaaNNumberValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random FAA N-number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated N-number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random FAA N-number using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated N-number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int digitCount = rng.Next(1, 6);
        int letterCount = digitCount == 5 ? 0 : digitCount == 4 ? rng.Next(0, 2) : rng.Next(0, 3);
        int total = 1 + digitCount + letterCount;
        if (destination.Length < total)
        {
            written = 0;
            return false;
        }
        destination[0] = 'N';
        destination[1] = (char)('1' + rng.Next(9));
        for (int i = 1; i < digitCount; i++)
            destination[1 + i] = (char)('0' + rng.Next(10));
        for (int i = 0; i < letterCount; i++)
            destination[1 + digitCount + i] = Letters[rng.Next(Letters.Length)];
        written = total;
        return true;
    }
}

