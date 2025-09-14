using System;

namespace Veritas.Government.MX.BCN;

/// <summary>Represents a validated Baja California driver licence number.</summary>
public readonly struct LicenciaDeConducirValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="LicenciaDeConducirValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public LicenciaDeConducirValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Baja California driver licence numbers.</summary>
public static class LicenciaDeConducir
{
    /// <summary>Attempts to validate the supplied Baja California driver licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<LicenciaDeConducirValue> result)
    {
        if (input.Length != 8)
        {
            result = new ValidationResult<LicenciaDeConducirValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> buffer = stackalloc char[8];
        for (int i = 0; i < 8; i++)
        {
            char ch = input[i];
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<LicenciaDeConducirValue>(false, default, ValidationError.Charset);
                return false;
            }
            buffer[i] = ch;
        }
        result = new ValidationResult<LicenciaDeConducirValue>(true, new LicenciaDeConducirValue(new string(buffer)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Baja California driver licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Baja California driver licence number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 8)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 8; i++)
            destination[i] = (char)('0' + rng.Next(10));
        written = 8;
        return true;
    }
}

