using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Energy;

/// <summary>Represents a validated Energy Identification Code (EIC).</summary>
public readonly struct EicValue
{
    /// <summary>Gets the normalized EIC code string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="EicValue"/> struct.</summary>
    /// <param name="value">The code string.</param>
    public EicValue(string value) => Value = value;
}

/// <summary>Provides validation for EIC codes.</summary>
public static class Eic
{
    /// <summary>Attempts to validate the supplied input as an EIC code.</summary>
    /// <param name="input">Candidate code to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation executed; the <see cref="ValidationResult{T}.IsValid"/> property indicates success.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EicValue> result)
    {
        Span<char> buffer = stackalloc char[16];
        if (!Normalize(input, buffer, out int len) || len != 16)
        {
            result = new ValidationResult<EicValue>(false, default, ValidationError.Length);
            return true;
        }
        if (!Iso7064.ValidateMod37_2(buffer))
        {
            result = new ValidationResult<EicValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(buffer);
        result = new ValidationResult<EicValue>(true, new EicValue(value), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random EIC code into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random EIC code using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 16) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> chars = destination[..16];
        for (int i = 0; i < 15; i++)
        {
            int v = rng.Next(36);
            chars[i] = v < 10 ? (char)('0' + v) : (char)('A' + v - 10);
        }
        chars[15] = Iso7064.ComputeCheckCharacterMod37_2(chars[..15]);
        written = 16;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int written)
    {
        written = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            char u = char.ToUpperInvariant(ch);
            if (!(char.IsDigit(u) || (u >= 'A' && u <= 'Z') || u == '*')) { written = 0; return false; }
            if (written >= dest.Length) { written = 0; return false; }
            dest[written++] = u;
        }
        return true;
    }
}

