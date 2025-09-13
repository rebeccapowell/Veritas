using System;
using Veritas;

namespace Veritas.Energy.GB;

/// <summary>Represents a validated UK Meter Point Reference Number (MPRN).</summary>
public readonly struct MprnValue
{
    /// <summary>Gets the normalized MPRN string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="MprnValue"/> struct.</summary>
    /// <param name="value">The identifier string.</param>
    public MprnValue(string value) => Value = value;
}

/// <summary>Provides validation for MPRN identifiers.</summary>
public static class Mprn
{
    /// <summary>Attempts to validate the supplied input as an MPRN.</summary>
    /// <param name="input">Candidate identifier to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MprnValue> result)
    {
        Span<char> digits = stackalloc char[10];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<MprnValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len < 6 || len > 10)
        {
            result = new ValidationResult<MprnValue>(false, default, ValidationError.Length);
            return false;
        }
        string value = new string(digits[..len]);
        result = new ValidationResult<MprnValue>(true, new MprnValue(value), ValidationError.None);
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
