using System;

namespace Veritas.Finance;

/// <summary>Represents a validated CHIPS participant identifier.</summary>
public readonly struct ChipsParticipantIdValue
{
    /// <summary>Gets the normalized CHIPS participant identifier.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="ChipsParticipantIdValue"/> struct.</summary>
    /// <param name="value">Normalized CHIPS participant identifier.</param>
    public ChipsParticipantIdValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for CHIPS participant identifiers.</summary>
public static class ChipsParticipantId
{
    /// <summary>Attempts to validate the supplied CHIPS participant identifier.</summary>
    /// <param name="input">Candidate CHIPS participant identifier.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ChipsParticipantIdValue> result)
    {
        Span<char> buf = stackalloc char[4];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (ch >= '0' && ch <= '9')
            {
                if (len == 4)
                {
                    result = new ValidationResult<ChipsParticipantIdValue>(false, default, ValidationError.Length);
                    return false;
                }
                buf[len++] = ch;
            }
            else
            {
                result = new ValidationResult<ChipsParticipantIdValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        if (len != 4)
        {
            result = new ValidationResult<ChipsParticipantIdValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<ChipsParticipantIdValue>(true, new ChipsParticipantIdValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random CHIPS participant identifier into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random CHIPS participant identifier using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated identifier.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 4)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        var value = rng.Next(0, 10000);
        value.TryFormat(destination, out written, "D4");
        return true;
    }
}

