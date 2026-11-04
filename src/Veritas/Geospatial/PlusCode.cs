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
        result = new ValidationResult<PlusCodeValue>(false, default, ValidationError.Format);
        if (input.Length > 4096) return false;
        Span<char> buf = stackalloc char[16];
        int len = 0, plusPos = -1;
        foreach (char c in input)
        {
            if (c == ' ') continue;
            if (len == buf.Length) return false;
            char u = char.ToUpperInvariant(c);
            if (u == '+')
            {
                if (plusPos >= 0) return false;
                plusPos = len;
            }
            else if (u != '0' && Alphabet.IndexOf(u) < 0) return false;
            buf[len++] = u;
        }
        if (len < 2 || plusPos < 0 || plusPos > 8 || plusPos % 2 != 0 || len - plusPos - 1 == 1) return false;
        int padding = buf[..len].IndexOf('0');
        if (padding >= 0)
        {
            if (plusPos != 8 || len != 9 || padding < 2 || padding % 2 != 0) return false;
            for (int i = padding; i < 8; i++) if (buf[i] != '0') return false;
        }
        if (plusPos == 8 && (Alphabet.IndexOf(buf[0]) >= 9 || Alphabet.IndexOf(buf[1]) >= 18)) return false;
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
        const int TotalLength = 11; // Ten significant characters and the separator.
        if (destination.Length < TotalLength)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 8; i++)
            destination[i] = Alphabet[rng.Next(Alphabet.Length)];
        destination[0] = Alphabet[rng.Next(9)];
        destination[1] = Alphabet[rng.Next(18)];
        destination[8] = '+';
        destination[9] = Alphabet[rng.Next(Alphabet.Length)];
        destination[10] = Alphabet[rng.Next(Alphabet.Length)];
        written = TotalLength;
        return true;
    }
}

