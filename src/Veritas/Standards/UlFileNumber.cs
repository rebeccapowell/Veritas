using System;

namespace Veritas.Standards;

/// <summary>Represents a validated UL file number.</summary>
public readonly struct UlFileNumberValue
{
    /// <summary>Gets the normalized UL file number.</summary>
    public string Value { get; }
    /// <summary>Initializes a new instance of the <see cref="UlFileNumberValue"/> struct.</summary>
    /// <param name="value">Normalized UL file number.</param>
    public UlFileNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for UL file numbers.</summary>
public static class UlFileNumber
{
    /// <summary>Attempts to validate the supplied UL file number.</summary>
    /// <param name="input">Candidate UL file number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<UlFileNumberValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (len == 0)
            {
                char u = char.ToUpperInvariant(ch);
                if (u != 'E')
                {
                    result = new ValidationResult<UlFileNumberValue>(false, default, ValidationError.Format);
                    return false;
                }
                buf[len++] = 'E';
            }
            else
            {
                if (ch < '0' || ch > '9')
                {
                    result = new ValidationResult<UlFileNumberValue>(false, default, ValidationError.Charset);
                    return false;
                }
                if (len >= 7)
                {
                    result = new ValidationResult<UlFileNumberValue>(false, default, ValidationError.Length);
                    return false;
                }
                buf[len++] = ch;
            }
        }
        if (len != 7)
        {
            result = new ValidationResult<UlFileNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<UlFileNumberValue>(true, new UlFileNumberValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random UL file number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated UL file number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random UL file number using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated UL file number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 7)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = 'E';
        for (int i = 1; i < 7; i++)
            destination[i] = (char)('0' + rng.Next(10));
        written = 7;
        return true;
    }
}

