using System;

namespace Veritas.Government.AR;

/// <summary>Represents a validated Argentine driver licence number.</summary>
public readonly struct LicenciaNacionalConducirValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="LicenciaNacionalConducirValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public LicenciaNacionalConducirValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Argentine driver licence numbers.</summary>
public static class LicenciaNacionalConducir
{
    /// <summary>Attempts to validate the supplied Argentine driver licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<LicenciaNacionalConducirValue> result)
    {
        if (input.Length != 7 && input.Length != 8)
        {
            result = new ValidationResult<LicenciaNacionalConducirValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> buf = stackalloc char[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            char ch = input[i];
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<LicenciaNacionalConducirValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[i] = ch;
        }
        result = new ValidationResult<LicenciaNacionalConducirValue>(true, new LicenciaNacionalConducirValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Argentine driver licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Argentine driver licence number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int len = 7 + rng.Next(2);
        if (destination.Length < len)
        {
            written = 0;
            return false;
        }
        for (int i = 0; i < len; i++)
            destination[i] = (char)('0' + rng.Next(10));
        written = len;
        return true;
    }
}

