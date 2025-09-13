using System;
using Veritas;

namespace Veritas.Energy.IT;

/// <summary>Represents a validated Italian PDR identifier.</summary>
public readonly struct PdrValue
{
    /// <summary>Gets the normalized PDR identifier string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="PdrValue"/> struct.</summary>
    /// <param name="value">The identifier string.</param>
    public PdrValue(string value) => Value = value;
}

/// <summary>Provides validation for PDR identifiers.</summary>
public static class Pdr
{
    /// <summary>Attempts to validate the supplied input as a PDR identifier.</summary>
    /// <param name="input">Candidate identifier to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PdrValue> result)
    {
        Span<char> digits = stackalloc char[14];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<PdrValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 14)
        {
            result = new ValidationResult<PdrValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<PdrValue>(true, new PdrValue(new string(digits)), ValidationError.None);
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
