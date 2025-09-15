using System;

namespace Veritas.Government.BR;

/// <summary>Represents a validated Brazilian CNH driver licence number.</summary>
public readonly struct CnhValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="CnhValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public CnhValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Brazilian CNH numbers.</summary>
public static class Cnh
{
    /// <summary>Attempts to validate the supplied CNH number.</summary>
    /// <param name="input">Candidate CNH.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CnhValue> result)
    {
        Span<int> digits = stackalloc int[11];
        if (input.Length != 11)
        {
            result = new ValidationResult<CnhValue>(false, default, ValidationError.Length);
            return false;
        }
        for (int i = 0; i < 11; i++)
        {
            char ch = input[i];
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<CnhValue>(false, default, ValidationError.Charset);
                return false;
            }
            digits[i] = ch - '0';
        }
        int d1 = 0, weight = 9;
        for (int i = 0; i < 9; i++) d1 += digits[i] * (weight--);
        d1 %= 11;
        d1 = d1 > 9 ? 0 : d1;
        int d2 = 0;
        weight = 1;
        for (int i = 0; i < 9; i++) d2 += digits[i] * (weight++);
        d2 = (d2 % 11);
        d2 = d2 > 9 ? 0 : d2;
        if (digits[9] != d1 || digits[10] != d2)
        {
            result = new ValidationResult<CnhValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<CnhValue>(true, new CnhValue(new string(input)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random CNH number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random CNH number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 11)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<int> digits = stackalloc int[11];
        for (int i = 0; i < 9; i++)
        {
            digits[i] = rng.Next(10);
            destination[i] = (char)('0' + digits[i]);
        }
        int d1 = 0, weight = 9;
        for (int i = 0; i < 9; i++) d1 += digits[i] * (weight--);
        d1 %= 11;
        d1 = d1 > 9 ? 0 : d1;
        digits[9] = d1;
        int d2 = 0;
        weight = 1;
        for (int i = 0; i < 9; i++) d2 += digits[i] * (weight++);
        d2 = (d2 % 11);
        d2 = d2 > 9 ? 0 : d2;
        digits[10] = d2;
        destination[9] = (char)('0' + d1);
        destination[10] = (char)('0' + d2);
        written = 11;
        return true;
    }
}

