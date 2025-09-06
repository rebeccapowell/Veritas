using System;
using Veritas;

namespace Veritas.Energy.DE;

/// <summary>Represents a validated German Meter Location (MeLo) identifier.</summary>
public readonly struct MeloValue
{
    /// <summary>Gets the normalized MeLo identifier string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="MeloValue"/> struct.</summary>
    /// <param name="value">The identifier string.</param>
    public MeloValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for MeLo identifiers.</summary>
public static class Melo
{
    /// <summary>Attempts to validate the supplied input as a MeLo identifier.</summary>
    /// <param name="input">Candidate identifier to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation executed; the <see cref="ValidationResult{T}.IsValid"/> property indicates success.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MeloValue> result)
    {
        Span<char> chars = stackalloc char[33];
        if (!Normalize(input, chars, out int len) || len != 33)
        {
            result = new ValidationResult<MeloValue>(false, default, ValidationError.Length);
            return true;
        }
        if (chars[0] != 'D' || chars[1] != 'E')
        {
            result = new ValidationResult<MeloValue>(false, default, ValidationError.Format);
            return true;
        }
        string value = new string(chars);
        result = new ValidationResult<MeloValue>(true, new MeloValue(value), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random MeLo identifier into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random MeLo identifier using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 33) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> chars = destination[..33];
        chars[0] = 'D';
        chars[1] = 'E';
        for (int i = 2; i < 33; i++)
        {
            int v = rng.Next(36);
            chars[i] = v < 10 ? (char)('0' + v) : (char)('A' + v - 10);
        }
        written = 33;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            char u = char.ToUpperInvariant(ch);
            if (!(char.IsDigit(u) || (u >= 'A' && u <= 'Z'))) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = u;
        }
        return true;
    }
}
