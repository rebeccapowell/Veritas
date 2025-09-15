using System;

namespace Veritas.Government.FR;

/// <summary>Represents a validated French driving licence number.</summary>
public readonly struct PermisDeConduireValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="PermisDeConduireValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public PermisDeConduireValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for French driving licence numbers.</summary>
public static class PermisDeConduire
{
    /// <summary>Attempts to validate the supplied French driving licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PermisDeConduireValue> result)
    {
        if (input.Length != 12)
        {
            result = new ValidationResult<PermisDeConduireValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> buf = stackalloc char[12];
        for (int i = 0; i < 12; i++)
        {
            char ch = input[i];
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<PermisDeConduireValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[i] = ch;
        }
        result = new ValidationResult<PermisDeConduireValue>(true, new PermisDeConduireValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random French driving licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random French driving licence number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 12)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 12; i++)
            destination[i] = (char)('0' + rng.Next(10));
        written = 12;
        return true;
    }
}

