using System;
using Veritas;

namespace Veritas.Energy.ES;

/// <summary>Represents a validated Spanish CUPS identifier.</summary>
public readonly struct CupsValue
{
    /// <summary>Gets the normalized CUPS identifier string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="CupsValue"/> struct.</summary>
    /// <param name="value">The identifier string.</param>
    public CupsValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for CUPS identifiers.</summary>
public static class Cups
{
    private const string Alphabet = "TRWAGMYFPDXBNJZSQVHLCKE";

    /// <summary>Attempts to validate the supplied input as a CUPS identifier.</summary>
    /// <param name="input">Candidate identifier to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation executed; the <see cref="ValidationResult{T}.IsValid"/> property indicates success.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CupsValue> result)
    {
        Span<char> buffer = stackalloc char[22];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<CupsValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 20 && len != 22)
        {
            result = new ValidationResult<CupsValue>(false, default, ValidationError.Length);
            return true;
        }
        if (buffer[0] != 'E' || buffer[1] != 'S')
        {
            result = new ValidationResult<CupsValue>(false, default, ValidationError.CountryRule);
            return true;
        }
        for (int i = 2; i < 18; i++)
        {
            if (!char.IsDigit(buffer[i]))
            {
                result = new ValidationResult<CupsValue>(false, default, ValidationError.Charset);
                return true;
            }
        }
        if (len == 22)
        {
            if (!char.IsDigit(buffer[20]))
            {
                result = new ValidationResult<CupsValue>(false, default, ValidationError.Charset);
                return true;
            }
            if ("FPRCXYZ".IndexOf(buffer[21]) < 0)
            {
                result = new ValidationResult<CupsValue>(false, default, ValidationError.Charset);
                return true;
            }
        }
        int rem = 0;
        for (int i = 2; i < 18; i++)
            rem = (rem * 10 + (buffer[i] - '0')) % 529;
        int check0 = rem / 23;
        int check1 = rem % 23;
        if (buffer[18] != Alphabet[check0] || buffer[19] != Alphabet[check1])
        {
            result = new ValidationResult<CupsValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(buffer[..len]);
        result = new ValidationResult<CupsValue>(true, new CupsValue(value), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random CUPS identifier into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random CUPS identifier using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 20)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = 'E';
        destination[1] = 'S';
        Span<char> digits = destination.Slice(2, 16);
        for (int i = 0; i < 16; i++)
            digits[i] = (char)('0' + rng.Next(10));
        int rem = 0;
        for (int i = 0; i < 16; i++)
            rem = (rem * 10 + (digits[i] - '0')) % 529;
        int check0 = rem / 23;
        int check1 = rem % 23;
        destination[18] = Alphabet[check0];
        destination[19] = Alphabet[check1];
        written = 20;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char u = char.ToUpperInvariant(ch);
            if (!(char.IsDigit(u) || (u >= 'A' && u <= 'Z'))) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = u;
        }
        return true;
    }
}
