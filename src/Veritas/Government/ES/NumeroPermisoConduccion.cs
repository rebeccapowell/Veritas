using System;

namespace Veritas.Government.ES;

/// <summary>Represents a validated Spanish driving licence number.</summary>
public readonly struct NumeroPermisoConduccionValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="NumeroPermisoConduccionValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public NumeroPermisoConduccionValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Spanish driving licence numbers.</summary>
public static class NumeroPermisoConduccion
{
    private const string Letters = "TRWAGMYFPDXBNJZSQVHLCKE";

    /// <summary>Attempts to validate the supplied Spanish driving licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NumeroPermisoConduccionValue> result)
    {
        Span<char> buf = stackalloc char[9];
        if (input.Length != 9)
        {
            result = new ValidationResult<NumeroPermisoConduccionValue>(false, default, ValidationError.Length);
            return false;
        }
        for (int i = 0; i < 8; i++)
        {
            char ch = input[i];
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<NumeroPermisoConduccionValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[i] = ch;
        }
        char letter = input[8];
        if (letter >= 'a' && letter <= 'z') letter = char.ToUpperInvariant(letter);
        if (letter < 'A' || letter > 'Z')
        {
            result = new ValidationResult<NumeroPermisoConduccionValue>(false, default, ValidationError.Charset);
            return false;
        }
        int num = 0;
        for (int i = 0; i < 8; i++) num = num * 10 + (buf[i] - '0');
        if (Letters[num % 23] != letter)
        {
            result = new ValidationResult<NumeroPermisoConduccionValue>(false, default, ValidationError.Checksum);
            return false;
        }
        buf[8] = letter;
        result = new ValidationResult<NumeroPermisoConduccionValue>(true, new NumeroPermisoConduccionValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Spanish driving licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Spanish driving licence number using the supplied options.</summary>
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
        int num = rng.Next(0, 100000000);
        for (int i = 7; i >= 0; i--)
        {
            destination[i] = (char)('0' + num % 10);
            num /= 10;
        }
        num = 0;
        for (int i = 0; i < 8; i++) num = num * 10 + (destination[i] - '0');
        destination[8] = Letters[num % 23];
        written = 9;
        return true;
    }
}

