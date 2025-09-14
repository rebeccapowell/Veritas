using System;

namespace Veritas.Geospatial;

/// <summary>Represents a validated Plus Code (Open Location Code).</summary>
public readonly struct PlusCodeValue
{
    /// <summary>Gets the normalized Plus Code.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="PlusCodeValue"/> struct.</summary>
    /// <param name="value">Normalized Plus Code.</param>
    public PlusCodeValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Plus Codes.</summary>
public static class PlusCode
{
    private const string Alphabet = "23456789CFGHJMPQRVWX";

    /// <summary>Attempts to validate the supplied input as a Plus Code.</summary>
    /// <param name="input">Candidate Plus Code to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PlusCodeValue> result)
    {
        Span<char> buf = stackalloc char[Math.Min(15, input.Length)];
        int len = 0;
        int plusPos = -1;
        foreach (var ch in input)
        {
            if (ch == ' ')
                continue;
            char u = char.ToUpperInvariant(ch);
            if (u == '+')
            {
                if (plusPos != -1)
                {
                    result = new ValidationResult<PlusCodeValue>(false, default, ValidationError.Format);
                    return false;
                }
                plusPos = len;
                buf[len++] = '+';
            }
            else
            {
                if (Alphabet.IndexOf(u) < 0)
                {
                    result = new ValidationResult<PlusCodeValue>(false, default, ValidationError.Charset);
                    return false;
                }
                if (len >= 15)
                {
                    result = new ValidationResult<PlusCodeValue>(false, default, ValidationError.Length);
                    return false;
                }
                buf[len++] = u;
            }
        }
        if (plusPos != 8 || plusPos == len - 1 || len < 9)
        {
            result = new ValidationResult<PlusCodeValue>(false, default, ValidationError.Format);
            return false;
        }
        result = new ValidationResult<PlusCodeValue>(true, new PlusCodeValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Plus Code into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated Plus Code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Plus Code using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated Plus Code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        const int TotalLength = 10; // 8 chars + '+' + 1 char
        if (destination.Length < TotalLength)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 8; i++)
            destination[i] = Alphabet[rng.Next(Alphabet.Length)];
        destination[8] = '+';
        destination[9] = Alphabet[rng.Next(Alphabet.Length)];
        written = TotalLength;
        return true;
    }
}

