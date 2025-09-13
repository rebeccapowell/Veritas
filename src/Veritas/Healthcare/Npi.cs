using System;
using Veritas.Algorithms;

namespace Veritas.Healthcare;

/// <summary>Represents a validated National Provider Identifier.</summary>
/// <example>Npi.TryValidate("1234567893", out var result);</example>
public readonly struct NpiValue
{
    /// <summary>The normalized NPI string.</summary>
    public string Value { get; }
    public NpiValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for U.S. National Provider Identifiers.</summary>
public static class Npi
{
    private const string Prefix = "80840";

    /// <summary>Validates the supplied NPI string.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NpiValue> result)
    {
        Span<char> digits = stackalloc char[10];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<NpiValue>(false, default, ValidationError.Charset); return false; }
            if (len >= 10) { result = new ValidationResult<NpiValue>(false, default, ValidationError.Length); return false; }
            digits[len++] = ch;
        }
        if (len != 10)
        {
            result = new ValidationResult<NpiValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> buffer = stackalloc char[Prefix.Length + 9];
        Prefix.AsSpan().CopyTo(buffer);
        digits[..9].CopyTo(buffer[Prefix.Length..]);
        int check = Luhn.ComputeCheckDigit(buffer);
        if (check != digits[9] - '0')
        {
            result = new ValidationResult<NpiValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<NpiValue>(true, new NpiValue(new string(digits)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a valid NPI into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a valid NPI using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 10)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 9; i++) destination[i] = (char)('0' + rng.Next(10));
        Span<char> buffer = stackalloc char[Prefix.Length + 9];
        Prefix.AsSpan().CopyTo(buffer);
        destination[..9].CopyTo(buffer[Prefix.Length..]);
        int check = Luhn.ComputeCheckDigit(buffer);
        destination[9] = (char)('0' + check);
        written = 10;
        return true;
    }
}
