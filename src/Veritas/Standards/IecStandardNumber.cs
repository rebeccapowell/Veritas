using System;
using System.Globalization;

namespace Veritas.Standards;

/// <summary>Represents a validated IEC standard number.</summary>
public readonly struct IecStandardNumberValue
{
    /// <summary>Gets the normalized IEC standard number.</summary>
    public string Value { get; }
    /// <summary>Initializes a new instance of the <see cref="IecStandardNumberValue"/> struct.</summary>
    /// <param name="value">Normalized IEC standard number.</param>
    public IecStandardNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for IEC standard numbers.</summary>
public static class IecStandardNumber
{
    /// <summary>Attempts to validate the supplied IEC standard number.</summary>
    /// <param name="input">Candidate IEC standard number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IecStandardNumberValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        int digitCount = 0;
        int yearCount = 0;
        bool afterColon = false;

        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (len < 3)
            {
                char u = char.ToUpperInvariant(ch);
                if ((len == 0 && u != 'I') || (len == 1 && u != 'E') || (len == 2 && u != 'C'))
                {
                    result = new ValidationResult<IecStandardNumberValue>(false, default, ValidationError.Format);
                    return false;
                }
                buf[len++] = u;
            }
            else if (!afterColon)
            {
                if (ch >= '0' && ch <= '9')
                {
                    if (digitCount >= 4)
                    {
                        result = new ValidationResult<IecStandardNumberValue>(false, default, ValidationError.Length);
                        return false;
                    }
                    buf[len++] = ch;
                    digitCount++;
                }
                else if (ch == ':' && digitCount == 4)
                {
                    buf[len++] = ':';
                    afterColon = true;
                }
                else
                {
                    result = new ValidationResult<IecStandardNumberValue>(false, default, ValidationError.Charset);
                    return false;
                }
            }
            else
            {
                if (ch < '0' || ch > '9')
                {
                    result = new ValidationResult<IecStandardNumberValue>(false, default, ValidationError.Charset);
                    return false;
                }
                if (yearCount >= 4)
                {
                    result = new ValidationResult<IecStandardNumberValue>(false, default, ValidationError.Length);
                    return false;
                }
                buf[len++] = ch;
                yearCount++;
            }
        }

        if (digitCount != 4 || (afterColon && yearCount != 4))
        {
            result = new ValidationResult<IecStandardNumberValue>(false, default, ValidationError.Length);
            return false;
        }

        result = new ValidationResult<IecStandardNumberValue>(true, new IecStandardNumberValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random IEC standard number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated IEC standard number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random IEC standard number using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated IEC standard number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        bool includeYear = rng.Next(2) == 0;
        int needed = 3 + 4 + (includeYear ? 1 + 4 : 0);
        if (destination.Length < needed)
        {
            written = 0;
            return false;
        }
        destination[0] = 'I';
        destination[1] = 'E';
        destination[2] = 'C';
        int num = rng.Next(1, 10000);
        var digits = num.ToString("D4", CultureInfo.InvariantCulture);
        digits.AsSpan().CopyTo(destination.Slice(3));
        if (includeYear)
        {
            destination[7] = ':';
            int year = rng.Next(1990, 2100);
            var yearDigits = year.ToString("D4", CultureInfo.InvariantCulture);
            yearDigits.AsSpan().CopyTo(destination.Slice(8));
        }
        written = needed;
        return true;
    }
}

