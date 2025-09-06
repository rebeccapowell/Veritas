using System;
using Veritas;

namespace Veritas.Telecom;

/// <summary>Represents a validated Organizationally Unique Identifier (OUI).</summary>
public readonly struct OuiValue
{
    /// <summary>Gets the normalized OUI string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="OuiValue"/> struct.</summary>
    /// <param name="value">The code string.</param>
    public OuiValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for OUI codes.</summary>
public static class Oui
{
    /// <summary>Attempts to validate the supplied input as an OUI code.</summary>
    /// <param name="input">Candidate code to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation executed; the <see cref="ValidationResult{T}.IsValid"/> property indicates success.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<OuiValue> result)
    {
        Span<char> buf = stackalloc char[6];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == '-' || ch == ':' || ch == '.') continue;
            char c = char.ToUpperInvariant(ch);
            if (!Uri.IsHexDigit(c))
            {
                result = new ValidationResult<OuiValue>(false, default, ValidationError.Charset);
                return true;
            }
            if (len >= 6)
            {
                result = new ValidationResult<OuiValue>(false, default, ValidationError.Length);
                return true;
            }
            buf[len++] = c;
        }
        if (len != 6)
        {
            result = new ValidationResult<OuiValue>(false, default, ValidationError.Length);
            return true;
        }
        result = new ValidationResult<OuiValue>(true, new OuiValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random OUI code into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random OUI code using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 6) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 6; i++)
        {
            int v = rng.Next(16);
            destination[i] = (char)(v < 10 ? '0' + v : 'A' + v - 10);
        }
        written = 6;
        return true;
    }
}

