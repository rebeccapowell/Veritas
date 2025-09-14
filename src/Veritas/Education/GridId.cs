using System;
using Veritas;

namespace Veritas.Education;

/// <summary>Canonical GRID identifier value.</summary>
public readonly struct GridIdValue { public string Value { get; } public GridIdValue(string v) => Value = v; }

/// <summary>Validation and generation for Global Research Identifier Database (GRID) identifiers.</summary>
public static class GridId
{
    private const string Prefix = "GRID.";

    /// <summary>Validates a GRID identifier.</summary>
    /// <param name="input">Input span to validate.</param>
    /// <param name="result">Normalized value when validation succeeds.</param>
    /// <returns><c>true</c> if the identifier is valid.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<GridIdValue> result)
    {
        if (!input.StartsWith(Prefix, StringComparison.Ordinal))
        {
            result = new ValidationResult<GridIdValue>(false, default, ValidationError.Format);
            return false;
        }
        if (input.Length != Prefix.Length + 9)
        {
            result = new ValidationResult<GridIdValue>(false, default, ValidationError.Length);
            return false;
        }
        var digits = input.Slice(Prefix.Length, 7);
        for (int i = 0; i < 7; i++)
            if (digits[i] < '0' || digits[i] > '9')
            {
                result = new ValidationResult<GridIdValue>(false, default, ValidationError.Charset);
                return false;
            }
        if (input[Prefix.Length + 7] != '.')
        {
            result = new ValidationResult<GridIdValue>(false, default, ValidationError.Format);
            return false;
        }
        char last = input[Prefix.Length + 8];
        if (!((last >= '0' && last <= '9') || (last >= 'A' && last <= 'Z')))
        {
            result = new ValidationResult<GridIdValue>(false, default, ValidationError.Charset);
            return false;
        }
        result = new ValidationResult<GridIdValue>(true, new GridIdValue(new string(input)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a random GRID identifier.</summary>
    /// <param name="destination">Buffer to receive the identifier.</param>
    /// <param name="written">Number of characters written.</param>
    /// <returns><c>true</c> if generation succeeded.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a random GRID identifier using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer to receive the identifier.</param>
    /// <param name="written">Number of characters written.</param>
    /// <returns><c>true</c> if generation succeeded.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < Prefix.Length + 9)
        {
            written = 0;
            return false;
        }
        Prefix.AsSpan().CopyTo(destination);
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 7; i++)
            destination[Prefix.Length + i] = (char)('0' + rng.Next(10));
        destination[Prefix.Length + 7] = '.';
        int val = rng.Next(36);
        destination[Prefix.Length + 8] = (char)(val < 10 ? '0' + val : 'A' + (val - 10));
        written = Prefix.Length + 9;
        return true;
    }
}

