using System;
using Veritas.Algorithms;
using Veritas.Tax.SE;

namespace Veritas.Government.SE;

/// <summary>Represents a validated Swedish driver licence number.</summary>
public readonly struct KorkortsnrValue
{
    /// <summary>Gets the normalized licence number.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="KorkortsnrValue"/> struct.</summary>
    /// <param name="value">Normalized licence number.</param>
    public KorkortsnrValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Swedish driver licence numbers.</summary>
public static class Korkortsnr
{
    /// <summary>Attempts to validate the supplied Swedish driver licence number.</summary>
    /// <param name="input">Candidate licence number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<KorkortsnrValue> result)
    {
        if (!Personnummer.TryValidate(input, out var pn) || pn.Value.Value.Length != 12)
        {
            result = new ValidationResult<KorkortsnrValue>(false, default, ValidationError.Format);
            return false;
        }
        result = new ValidationResult<KorkortsnrValue>(true, new KorkortsnrValue(pn.Value.Value), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random Swedish driver licence number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random Swedish driver licence number using the supplied options.</summary>
    /// <param name="options">Generation options controlling randomness.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int year = 1950 + rng.Next(50); // 1950-1999
        int month = 1 + rng.Next(12);
        int day = 1 + rng.Next(28);
        int seq = rng.Next(1000);
        return GenerateInternal(year, month, day, seq, destination, out written);
    }

    private static bool GenerateInternal(int year, int month, int day, int seq, Span<char> dest, out int written)
    {
        Span<char> digits = stackalloc char[11];
        digits[0] = (char)('0' + (year / 1000));
        digits[1] = (char)('0' + (year / 100 % 10));
        digits[2] = (char)('0' + (year / 10 % 10));
        digits[3] = (char)('0' + (year % 10));
        digits[4] = (char)('0' + (month / 10));
        digits[5] = (char)('0' + (month % 10));
        digits[6] = (char)('0' + (day / 10));
        digits[7] = (char)('0' + (day % 10));
        digits[8] = (char)('0' + (seq / 100));
        digits[9] = (char)('0' + (seq / 10 % 10));
        digits[10] = (char)('0' + (seq % 10));
        int check = Luhn.ComputeCheckDigit(digits[2..11]);
        if (dest.Length < 12)
        {
            written = 0;
            return false;
        }
        digits[..11].CopyTo(dest);
        dest[11] = (char)('0' + check);
        written = 12;
        return true;
    }
}

