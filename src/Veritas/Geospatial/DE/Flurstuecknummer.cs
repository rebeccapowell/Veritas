using System;

namespace Veritas.Geospatial.DE;

/// <summary>Represents a validated German land parcel number (Flurstücknummer).</summary>
public readonly struct FlurstuecknummerValue
{
    /// <summary>Gets the normalized parcel number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="FlurstuecknummerValue"/> struct.</summary>
    /// <param name="value">Normalized parcel number.</param>
    public FlurstuecknummerValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for German land parcel numbers.</summary>
public static class Flurstuecknummer
{
    /// <summary>Attempts to validate the supplied input as a Flurstücknummer.</summary>
    /// <param name="input">Candidate parcel number to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<FlurstuecknummerValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        int digitsBefore = 0;
        int digitsAfter = 0;
        bool slashSeen = false;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (ch == '/')
            {
                if (slashSeen)
                {
                    result = new ValidationResult<FlurstuecknummerValue>(false, default, ValidationError.Format);
                    return false;
                }
                slashSeen = true;
                buf[len++] = '/';
                continue;
            }
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<FlurstuecknummerValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[len++] = ch;
            if (!slashSeen) digitsBefore++; else digitsAfter++;
        }
        if (digitsBefore != 11 || (slashSeen && (digitsAfter < 1 || digitsAfter > 4)))
        {
            result = new ValidationResult<FlurstuecknummerValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<FlurstuecknummerValue>(true, new FlurstuecknummerValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Flurstücknummer into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated parcel number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Flurstücknummer using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated parcel number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        bool withNenner = rng.Next(2) == 0;
        int needed = 11 + (withNenner ? 5 : 0);
        if (destination.Length < needed)
        {
            written = 0;
            return false;
        }
        for (int i = 0; i < 11; i++)
            destination[i] = (char)('0' + rng.Next(10));
        if (withNenner)
        {
            destination[11] = '/';
            for (int i = 0; i < 4; i++)
                destination[12 + i] = (char)('0' + rng.Next(10));
        }
        written = needed;
        return true;
    }
}

