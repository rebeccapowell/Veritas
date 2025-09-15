using System;

namespace Veritas.Government.DE;

/// <summary>Represents a validated German driver licence number (Führerscheinnummer).</summary>
public readonly struct FuehrerscheinnummerValue
{
    /// <summary>Gets the normalized driver licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="FuehrerscheinnummerValue"/> struct.</summary>
    /// <param name="value">Normalized driver licence number.</param>
    public FuehrerscheinnummerValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for German driver licence numbers.</summary>
public static class Fuehrerscheinnummer
{
    /// <summary>Attempts to validate the supplied German driver licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<FuehrerscheinnummerValue> result)
    {
        if (input.Length < 10 || input.Length > 12)
        {
            result = new ValidationResult<FuehrerscheinnummerValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> buf = input.Length <= 12 ? stackalloc char[input.Length] : stackalloc char[12];
        for (int i = 0; i < input.Length; i++)
        {
            char ch = input[i];
            if (ch >= 'a' && ch <= 'z') ch = (char)(ch - 32);
            if (!(ch >= 'A' && ch <= 'Z') && (ch < '0' || ch > '9'))
            {
                result = new ValidationResult<FuehrerscheinnummerValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[i] = ch;
        }
        result = new ValidationResult<FuehrerscheinnummerValue>(true, new FuehrerscheinnummerValue(new string(buf[..input.Length])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random German driver licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random German driver licence number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int len = 10 + rng.Next(3); // 10-12
        if (destination.Length < len)
        {
            written = 0;
            return false;
        }
        for (int i = 0; i < len; i++)
        {
            int v = rng.Next(36);
            destination[i] = v < 10 ? (char)('0' + v) : (char)('A' + v - 10);
        }
        written = len;
        return true;
    }
}

