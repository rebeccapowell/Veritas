using System;

namespace Veritas.Government.IT;

/// <summary>Represents a validated Italian driving licence number.</summary>
public readonly struct PatenteDiGuidaValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="PatenteDiGuidaValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public PatenteDiGuidaValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Italian driving licence numbers.</summary>
public static class PatenteDiGuida
{
    /// <summary>Attempts to validate the supplied Italian driving licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PatenteDiGuidaValue> result)
    {
        if (input.Length != 10)
        {
            result = new ValidationResult<PatenteDiGuidaValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> buf = stackalloc char[10];
        for (int i = 0; i < 10; i++)
        {
            char ch = input[i];
            if (ch >= 'a' && ch <= 'z') ch = (char)(ch - 32);
            if (!(ch >= 'A' && ch <= 'Z') && (ch < '0' || ch > '9'))
            {
                result = new ValidationResult<PatenteDiGuidaValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[i] = ch;
        }
        result = new ValidationResult<PatenteDiGuidaValue>(true, new PatenteDiGuidaValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Italian driving licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Italian driving licence number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 10)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 10; i++)
        {
            int v = rng.Next(36);
            destination[i] = v < 10 ? (char)('0' + v) : (char)('A' + v - 10);
        }
        written = 10;
        return true;
    }
}

