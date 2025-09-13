using System;
using Veritas;

namespace Veritas.Telecom;

/// <summary>Represents a validated MAC address.</summary>
public readonly struct MacValue
{
    /// <summary>Gets the normalized MAC address string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="MacValue"/> struct.</summary>
    /// <param name="value">The address string.</param>
    public MacValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for MAC addresses.</summary>
public static class Mac
{
    /// <summary>Attempts to validate the supplied input as a MAC address.</summary>
    /// <param name="input">Candidate address to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MacValue> result)
    {
        Span<char> buf = stackalloc char[12];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == '-' || ch == ':' || ch == '.') continue;
            char c = char.ToUpperInvariant(ch);
            if (!Uri.IsHexDigit(c)) { result = new ValidationResult<MacValue>(false, default, ValidationError.Charset); return false; }
            if (len >= 12) { result = new ValidationResult<MacValue>(false, default, ValidationError.Length); return false; }
            buf[len++] = c;
        }
        if (len != 12) { result = new ValidationResult<MacValue>(false, default, ValidationError.Length); return false; }
        result = new ValidationResult<MacValue>(true, new MacValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random MAC address into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated address.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random MAC address using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated address.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 12)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        const string hex = "0123456789ABCDEF";
        for (int i = 0; i < 12; i++)
            destination[i] = hex[rng.Next(16)];
        written = 12;
        return true;
    }
}

