using System;
using System.Globalization;

namespace Veritas.Standards;

/// <summary>Represents a validated RFC number.</summary>
public readonly struct RfcNumberValue
{
    /// <summary>Gets the normalized RFC number string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="RfcNumberValue"/> struct.</summary>
    /// <param name="value">Normalized RFC number.</param>
    public RfcNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for RFC numbers.</summary>
public static class RfcNumber
{
    /// <summary>Attempts to validate the supplied input as an RFC number.</summary>
    /// <param name="input">Candidate RFC number to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<RfcNumberValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (len < 3)
            {
                char u = char.ToUpperInvariant(ch);
                if ((len == 0 && u != 'R') || (len == 1 && u != 'F') || (len == 2 && u != 'C'))
                {
                    result = new ValidationResult<RfcNumberValue>(false, default, ValidationError.Format);
                    return false;
                }
                buf[len++] = u;
            }
            else
            {
                if (ch < '0' || ch > '9')
                {
                    result = new ValidationResult<RfcNumberValue>(false, default, ValidationError.Charset);
                    return false;
                }
                buf[len++] = ch;
            }
        }
        if (len <= 3)
        {
            result = new ValidationResult<RfcNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        var normalized = new string(buf[..len]);
        result = new ValidationResult<RfcNumberValue>(true, new RfcNumberValue(normalized), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random RFC number into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated RFC number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random RFC number using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated RFC number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int value = rng.Next(1, 100000); // up to 5 digits
        var digits = value.ToString(CultureInfo.InvariantCulture);
        int needed = 3 + digits.Length;
        if (destination.Length < needed)
        {
            written = 0;
            return false;
        }
        destination[0] = 'R';
        destination[1] = 'F';
        destination[2] = 'C';
        digits.AsSpan().CopyTo(destination.Slice(3));
        written = needed;
        return true;
    }
}
