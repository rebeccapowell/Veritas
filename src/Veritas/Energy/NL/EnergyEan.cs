using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Energy.NL;

/// <summary>Represents a validated Dutch energy EAN code.</summary>
public readonly struct EnergyEanValue
{
    /// <summary>Gets the normalized EAN code string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="EnergyEanValue"/> struct.</summary>
    /// <param name="value">The code string.</param>
    public EnergyEanValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Dutch energy EAN codes.</summary>
public static class EnergyEan
{
    /// <summary>Attempts to validate the supplied input as an energy EAN code.</summary>
    /// <param name="input">Candidate code to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EnergyEanValue> result)
    {
        Span<char> digits = stackalloc char[18];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<EnergyEanValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 18)
        {
            result = new ValidationResult<EnergyEanValue>(false, default, ValidationError.Length);
            return false;
        }
        if (!Gs1.Validate(digits))
        {
            result = new ValidationResult<EnergyEanValue>(false, default, ValidationError.Checksum);
            return false;
        }
        string value = new string(digits);
        result = new ValidationResult<EnergyEanValue>(true, new EnergyEanValue(value), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random energy EAN code into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random energy EAN code using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 18)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = stackalloc char[17];
        for (int i = 0; i < 17; i++)
            digits[i] = (char)('0' + rng.Next(10));
        int check = Gs1.ComputeCheckDigit(digits);
        digits.CopyTo(destination);
        destination[17] = (char)('0' + check);
        written = 18;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}
