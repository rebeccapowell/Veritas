using System;
using Veritas;

namespace Veritas.Education;

/// <summary>Canonical ROR identifier value.</summary>
public readonly struct RorIdValue { public string Value { get; } public RorIdValue(string v) => Value = v; }

/// <summary>Validation and generation for Research Organization Registry (ROR) identifiers.</summary>
public static class RorId
{
    private const string Prefix = "https://ror.org/";

    /// <summary>Validates a ROR identifier.</summary>
    /// <param name="input">Input span to validate.</param>
    /// <param name="result">Normalized value when validation succeeds.</param>
    /// <returns><c>true</c> if the identifier is valid.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<RorIdValue> result)
    {
        if (!input.StartsWith(Prefix, StringComparison.Ordinal))
        {
            result = new ValidationResult<RorIdValue>(false, default, ValidationError.Format);
            return false;
        }
        var body = input[Prefix.Length..];
        if (body.Length != 9)
        {
            result = new ValidationResult<RorIdValue>(false, default, ValidationError.Length);
            return false;
        }
        foreach (var ch in body)
        {
            if (!((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z')))
            {
                result = new ValidationResult<RorIdValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        result = new ValidationResult<RorIdValue>(true, new RorIdValue(new string(input)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a random ROR identifier.</summary>
    /// <param name="destination">Buffer to receive the identifier.</param>
    /// <param name="written">Number of characters written.</param>
    /// <returns><c>true</c> if generation succeeded.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a random ROR identifier using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer to receive the identifier.</param>
    /// <param name="written">Number of characters written.</param>
    /// <returns><c>true</c> if generation succeeded.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";
        if (destination.Length < Prefix.Length + 9)
        {
            written = 0;
            return false;
        }
        Prefix.AsSpan().CopyTo(destination);
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 9; i++)
            destination[Prefix.Length + i] = Alphabet[rng.Next(Alphabet.Length)];
        written = Prefix.Length + 9;
        return true;
    }
}

