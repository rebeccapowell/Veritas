using System;
using Veritas;

namespace Veritas.Telecom;

/// <summary>Represents a validated International Mobile Subscriber Identity (IMSI).</summary>
public readonly struct ImsiValue
{
    /// <summary>Gets the normalized IMSI string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="ImsiValue"/> struct.</summary>
    /// <param name="value">The code string.</param>
    public ImsiValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for IMSI numbers.</summary>
public static class Imsi
{
    /// <summary>Attempts to validate the supplied input as an IMSI.</summary>
    /// <param name="input">Candidate code to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ImsiValue> result)
    {
        Span<char> digits = stackalloc char[15];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<ImsiValue>(false, default, ValidationError.Charset);
                return false;
            }
            if (len >= 15)
            {
                result = new ValidationResult<ImsiValue>(false, default, ValidationError.Length);
                return false;
            }
            digits[len++] = ch;
        }
        if (len != 15)
        {
            result = new ValidationResult<ImsiValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<ImsiValue>(true, new ImsiValue(new string(digits)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random IMSI into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random IMSI using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 15)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        var (mcc, mncs) = _samples[rng.Next(_samples.Length)];
        mcc.AsSpan().CopyTo(destination);
        var mnc = mncs[rng.Next(mncs.Length)];
        mnc.AsSpan().CopyTo(destination.Slice(3));
        int pos = 3 + mnc.Length;
        for (int i = pos; i < 15; i++)
            destination[i] = (char)('0' + rng.Next(10));
        written = 15;
        return true;
    }

    private static readonly (string Mcc, string[] Mncs)[] _samples = new[]
    {
        ("310", new[] { "260", "410" }), // United States
        ("262", new[] { "01", "02" }),   // Germany
        ("234", new[] { "10", "15" })    // United Kingdom
    };
}

