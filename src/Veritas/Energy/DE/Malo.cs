using System;
using Veritas;

namespace Veritas.Energy.DE;

/// <summary>Represents a validated German Market Location (MaLo) identifier.</summary>
public readonly struct MaloValue
{
    /// <summary>Gets the normalized MaLo identifier string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="MaloValue"/> struct.</summary>
    /// <param name="value">The identifier string.</param>
    public MaloValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for MaLo identifiers.</summary>
public static class Malo
{
    /// <summary>Attempts to validate the supplied input as a MaLo identifier.</summary>
    /// <param name="input">Candidate identifier to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation executed; the <see cref="ValidationResult{T}.IsValid"/> property indicates success.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MaloValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len) || len != 11)
        {
            result = new ValidationResult<MaloValue>(false, default, ValidationError.Length);
            return true;
        }
        int sumOdd = 0, sumEven = 0;
        for (int i = 0; i < 10; i++)
        {
            int d = digits[i] - '0';
            if ((i & 1) == 0) sumOdd += d; else sumEven += d;
        }
        int total = sumOdd + 2 * sumEven;
        int check = (10 - total % 10) % 10;
        if (digits[10] != (char)('0' + check))
        {
            result = new ValidationResult<MaloValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(digits);
        result = new ValidationResult<MaloValue>(true, new MaloValue(value), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random MaLo identifier into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random MaLo identifier using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 11) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..11];
        digits[0] = (char)('1' + rng.Next(9));
        for (int i = 1; i < 10; i++)
            digits[i] = (char)('0' + rng.Next(10));
        int sumOdd = 0, sumEven = 0;
        for (int i = 0; i < 10; i++)
        {
            int d = digits[i] - '0';
            if ((i & 1) == 0) sumOdd += d; else sumEven += d;
        }
        int total = sumOdd + 2 * sumEven;
        digits[10] = (char)('0' + ((10 - total % 10) % 10));
        written = 11;
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
