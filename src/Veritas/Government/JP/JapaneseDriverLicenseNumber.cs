using System;

namespace Veritas.Government.JP;

/// <summary>Represents a validated Japanese driver licence number.</summary>
public readonly struct JapaneseDriverLicenseNumberValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="JapaneseDriverLicenseNumberValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public JapaneseDriverLicenseNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Japanese driver licence numbers.</summary>
public static class JapaneseDriverLicenseNumber
{
    private static readonly int[] Weights = { 2, 3, 4, 5, 6, 7, 2, 3, 4, 5, 6 };

    /// <summary>Attempts to validate the supplied Japanese driver licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<JapaneseDriverLicenseNumberValue> result)
    {
        Span<int> digits = stackalloc int[12];
        if (input.Length != 12)
        {
            result = new ValidationResult<JapaneseDriverLicenseNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        for (int i = 0; i < 12; i++)
        {
            char ch = input[i];
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<JapaneseDriverLicenseNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
            digits[i] = ch - '0';
        }
        int sum = 0;
        for (int i = 0; i < 11; i++)
            sum += digits[i] * Weights[i];
        int check = (11 - (sum % 11)) % 10;
        if (digits[11] != check)
        {
            result = new ValidationResult<JapaneseDriverLicenseNumberValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<JapaneseDriverLicenseNumberValue>(true, new JapaneseDriverLicenseNumberValue(new string(input)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Japanese driver licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Japanese driver licence number using the supplied options.</summary>
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
        Span<int> digits = stackalloc int[12];
        for (int i = 0; i < 11; i++)
        {
            digits[i] = rng.Next(10);
            destination[i] = (char)('0' + digits[i]);
        }
        int sum = 0;
        for (int i = 0; i < 11; i++)
            sum += digits[i] * Weights[i];
        int check = (11 - (sum % 11)) % 10;
        destination[11] = (char)('0' + check);
        written = 12;
        return true;
    }
}

