using System;
using Veritas;

namespace Veritas.Energy.FR;

/// <summary>Represents a validated French PRM identifier.</summary>
public readonly struct PrmValue
{
    /// <summary>Gets the normalized PRM identifier string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="PrmValue"/> struct.</summary>
    /// <param name="value">The identifier string.</param>
    public PrmValue(string value) => Value = value;
}

/// <summary>Provides validation for PRM identifiers.</summary>
public static class Prm
{
    /// <summary>Attempts to validate the supplied input as a PRM identifier.</summary>
    /// <param name="input">Candidate identifier to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PrmValue> result)
    {
        Span<char> digits = stackalloc char[14];
        if (!Normalize(input, digits, out int len) || len != 14)
        {
            result = new ValidationResult<PrmValue>(false, default, ValidationError.Length);
            return false;
        }
        string value = new string(digits);
        result = new ValidationResult<PrmValue>(true, new PrmValue(value), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}
