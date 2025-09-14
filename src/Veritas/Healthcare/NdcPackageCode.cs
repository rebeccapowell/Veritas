using System;

namespace Veritas.Healthcare;

/// <summary>Represents a validated NDC package code.</summary>
public readonly struct NdcPackageCodeValue
{
    /// <summary>Gets the normalized NDC package code digits.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="NdcPackageCodeValue"/> struct.</summary>
    /// <param name="value">Normalized digits.</param>
    public NdcPackageCodeValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for NDC package codes.</summary>
public static class NdcPackageCode
{
    /// <summary>Attempts to validate the supplied NDC package code.</summary>
    /// <param name="input">Candidate code.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NdcPackageCodeValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<NdcPackageCodeValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[len++] = ch;
        }
        if (len != 12)
        {
            result = new ValidationResult<NdcPackageCodeValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<NdcPackageCodeValue>(true, new NdcPackageCodeValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random NDC package code into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated digits.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random NDC package code using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated digits.</param>
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
        int labeler = rng.Next(0, 100000);
        int product = rng.Next(0, 1000);
        int package = rng.Next(0, 100);
        int suffix = rng.Next(0, 100);
        var s = labeler.ToString("00000") + product.ToString("000") + package.ToString("00") + suffix.ToString("00");
        s.AsSpan().CopyTo(destination);
        written = 12;
        return true;
    }
}

