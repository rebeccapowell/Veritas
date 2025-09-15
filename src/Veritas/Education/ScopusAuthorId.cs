using System;
using Veritas;

namespace Veritas.Education;

/// <summary>Canonical Scopus Author ID value.</summary>
public readonly struct ScopusAuthorIdValue { public string Value { get; } public ScopusAuthorIdValue(string v) => Value = v; }

/// <summary>Validation and generation for Scopus Author identifiers.</summary>
public static class ScopusAuthorId
{
    /// <summary>Validates a Scopus Author ID.</summary>
    /// <param name="input">Input span to validate.</param>
    /// <param name="result">Normalized value when validation succeeds.</param>
    /// <returns><c>true</c> if the identifier is valid.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ScopusAuthorIdValue> result)
    {
        Span<char> digits = stackalloc char[11];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<ScopusAuthorIdValue>(false, default, ValidationError.Charset);
                return false;
            }
            if (len >= 11)
            {
                result = new ValidationResult<ScopusAuthorIdValue>(false, default, ValidationError.Length);
                return false;
            }
            digits[len++] = ch;
        }
        if (len != 11)
        {
            result = new ValidationResult<ScopusAuthorIdValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<ScopusAuthorIdValue>(true, new ScopusAuthorIdValue(new string(digits)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a random Scopus Author ID.</summary>
    /// <param name="destination">Buffer to receive the identifier.</param>
    /// <param name="written">Number of characters written.</param>
    /// <returns><c>true</c> if generation succeeded.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a random Scopus Author ID using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer to receive the identifier.</param>
    /// <param name="written">Number of characters written.</param>
    /// <returns><c>true</c> if generation succeeded.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 11)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 11; i++)
            destination[i] = (char)('0' + rng.Next(10));
        written = 11;
        return true;
    }
}

