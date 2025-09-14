using System;

namespace Veritas.Government.NL;

/// <summary>Represents a validated Dutch driving licence number.</summary>
public readonly struct RijbewijsnummerValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="RijbewijsnummerValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public RijbewijsnummerValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Dutch driving licence numbers.</summary>
public static class Rijbewijsnummer
{
    /// <summary>Attempts to validate the supplied Dutch driving licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<RijbewijsnummerValue> result)
    {
        if (input.Length != 9)
        {
            result = new ValidationResult<RijbewijsnummerValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> buf = stackalloc char[9];
        for (int i = 0; i < 2; i++)
        {
            char ch = input[i];
            if (!(ch >= 'A' && ch <= 'Z') && !(ch >= 'a' && ch <= 'z'))
            {
                result = new ValidationResult<RijbewijsnummerValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[i] = char.ToUpperInvariant(ch);
        }
        for (int i = 2; i < 8; i++)
        {
            char ch = input[i];
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<RijbewijsnummerValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[i] = ch;
        }
        char last = input[8];
        if (!(last >= 'A' && last <= 'Z') && !(last >= 'a' && last <= 'z'))
        {
            result = new ValidationResult<RijbewijsnummerValue>(false, default, ValidationError.Charset);
            return false;
        }
        buf[8] = char.ToUpperInvariant(last);
        result = new ValidationResult<RijbewijsnummerValue>(true, new RijbewijsnummerValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Dutch driving licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Dutch driving licence number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 9)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = (char)('A' + rng.Next(26));
        destination[1] = (char)('A' + rng.Next(26));
        for (int i = 2; i < 8; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[8] = (char)('A' + rng.Next(26));
        written = 9;
        return true;
    }
}

