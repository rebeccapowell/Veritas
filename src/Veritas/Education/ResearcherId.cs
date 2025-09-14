using System;
using Veritas;

namespace Veritas.Education;

/// <summary>Canonical ResearcherID value.</summary>
public readonly struct ResearcherIdValue { public string Value { get; } public ResearcherIdValue(string v) => Value = v; }

/// <summary>Validation and generation for ResearcherID identifiers.</summary>
public static class ResearcherId
{
    /// <summary>Validates a ResearcherID.</summary>
    /// <param name="input">Input span to validate.</param>
    /// <param name="result">Normalized value when validation succeeds.</param>
    /// <returns><c>true</c> if the identifier is valid.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ResearcherIdValue> result)
    {
        if (input.Length != 11)
        {
            result = new ValidationResult<ResearcherIdValue>(false, default, ValidationError.Length);
            return false;
        }
        char c0 = input[0];
        if (c0 < 'A' || c0 > 'Z')
        {
            result = new ValidationResult<ResearcherIdValue>(false, default, ValidationError.Charset);
            return false;
        }
        if (input[1] != '-' || input[6] != '-')
        {
            result = new ValidationResult<ResearcherIdValue>(false, default, ValidationError.Format);
            return false;
        }
        for (int i = 2; i < 6; i++)
            if (input[i] < '0' || input[i] > '9')
            {
                result = new ValidationResult<ResearcherIdValue>(false, default, ValidationError.Charset);
                return false;
            }
        for (int i = 7; i < 11; i++)
            if (input[i] < '0' || input[i] > '9')
            {
                result = new ValidationResult<ResearcherIdValue>(false, default, ValidationError.Charset);
                return false;
            }
        int year = (input[7] - '0') * 1000 + (input[8] - '0') * 100 + (input[9] - '0') * 10 + (input[10] - '0');
        if (year < 1900 || year > 2099)
        {
            result = new ValidationResult<ResearcherIdValue>(false, default, ValidationError.Range);
            return false;
        }
        result = new ValidationResult<ResearcherIdValue>(true, new ResearcherIdValue(new string(input)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a random ResearcherID.</summary>
    /// <param name="destination">Buffer to receive the identifier.</param>
    /// <param name="written">Number of characters written.</param>
    /// <returns><c>true</c> if generation succeeded.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a random ResearcherID using the supplied options.</summary>
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
        destination[0] = (char)('A' + rng.Next(26));
        destination[1] = '-';
        for (int i = 0; i < 4; i++)
            destination[2 + i] = (char)('0' + rng.Next(10));
        destination[6] = '-';
        int year = rng.Next(1900, 2100);
        destination[7] = (char)('0' + year / 1000);
        destination[8] = (char)('0' + (year / 100) % 10);
        destination[9] = (char)('0' + (year / 10) % 10);
        destination[10] = (char)('0' + year % 10);
        written = 11;
        return true;
    }
}

