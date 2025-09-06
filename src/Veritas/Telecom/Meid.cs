using System;
using Veritas;

namespace Veritas.Telecom;

/// <summary>Represents a validated Mobile Equipment Identifier (MEID).</summary>
public readonly struct MeidValue
{
    /// <summary>Gets the normalized MEID string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="MeidValue"/> struct.</summary>
    /// <param name="value">The code string.</param>
    public MeidValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for MEID numbers.</summary>
public static class Meid
{
    /// <summary>Attempts to validate the supplied input as a MEID.</summary>
    /// <param name="input">Candidate code to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation executed; the <see cref="ValidationResult{T}.IsValid"/> property indicates success.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MeidValue> result)
    {
        Span<char> buf = stackalloc char[18];
        if (!Normalize(input, buf, out int len))
        {
            result = new ValidationResult<MeidValue>(false, default, ValidationError.Charset);
            return true;
        }
        if (len == 14)
        {
            for (int i = 0; i < 14; i++)
            {
                char c = buf[i];
                if (!Uri.IsHexDigit(c))
                {
                    result = new ValidationResult<MeidValue>(false, default, ValidationError.Charset);
                    return true;
                }
                buf[i] = char.ToUpperInvariant(c);
            }
            result = new ValidationResult<MeidValue>(true, new MeidValue(new string(buf[..14])), ValidationError.None);
            return true;
        }
        else if (len == 18)
        {
            for (int i = 0; i < 18; i++)
            {
                if (!char.IsDigit(buf[i]))
                {
                    result = new ValidationResult<MeidValue>(false, default, ValidationError.Charset);
                    return true;
                }
            }
            int check = Luhn(buf[..17]);
            if (buf[17] != (char)('0' + check))
            {
                result = new ValidationResult<MeidValue>(false, default, ValidationError.Checksum);
                return true;
            }
            result = new ValidationResult<MeidValue>(true, new MeidValue(new string(buf[..18])), ValidationError.None);
            return true;
        }
        result = new ValidationResult<MeidValue>(false, default, ValidationError.Length);
        return true;
    }

    /// <summary>Attempts to generate a random MEID into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random MEID using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 14) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 14; i++)
        {
            int v = rng.Next(16);
            destination[i] = (char)(v < 10 ? '0' + v : 'A' + v - 10);
        }
        written = 14;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-' || ch == '.') continue;
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }

    private static int Luhn(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        bool dbl = true;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i] - '0';
            if (dbl)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            dbl = !dbl;
        }
        return (10 - sum % 10) % 10;
    }
}

