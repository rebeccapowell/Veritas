using System;

namespace Veritas.Healthcare;

/// <summary>Represents a validated National Drug Code (NDC).</summary>
public readonly struct NdcValue
{
    /// <summary>Gets the normalized NDC digits.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="NdcValue"/> struct.</summary>
    /// <param name="value">Normalized digits.</param>
    public NdcValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for U.S. National Drug Codes.</summary>
public static class Ndc
{
    /// <summary>Attempts to validate the supplied NDC.</summary>
    /// <param name="input">Candidate NDC.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NdcValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<NdcValue>(false, default, ValidationError.Charset);
                return false;
            }
            buf[len++] = ch;
        }
        if (len != 10 && len != 11)
        {
            result = new ValidationResult<NdcValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<NdcValue>(true, new NdcValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random NDC into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated digits.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random NDC using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated digits.</param>
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
        int labeler = rng.Next(0, 100000);
        int product = rng.Next(0, 10000);
        int package = rng.Next(0, 100);
        var s = labeler.ToString("00000") + product.ToString("0000") + package.ToString("00");
        s.AsSpan().CopyTo(destination);
        written = 11;
        return true;
    }
}

