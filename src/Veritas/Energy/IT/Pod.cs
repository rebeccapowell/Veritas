using System;
using Veritas;

namespace Veritas.Energy.IT;

/// <summary>Represents a validated Italian POD identifier.</summary>
public readonly struct PodValue
{
    /// <summary>Gets the normalized POD identifier string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="PodValue"/> struct.</summary>
    /// <param name="value">The identifier string.</param>
    public PodValue(string value) => Value = value;
}

/// <summary>Provides validation for POD identifiers.</summary>
public static class Pod
{
    /// <summary>Attempts to validate the supplied input as a POD identifier.</summary>
    /// <param name="input">Candidate identifier to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation executed; the <see cref="ValidationResult{T}.IsValid"/> property indicates success.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PodValue> result)
    {
        Span<char> chars = stackalloc char[16];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<PodValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 16 || chars[0] != 'I' || chars[1] != 'T')
        {
            result = new ValidationResult<PodValue>(false, default, ValidationError.Format);
            return true;
        }
        result = new ValidationResult<PodValue>(true, new PodValue(new string(chars)), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            char c = char.ToUpperInvariant(ch);
            if (!char.IsLetterOrDigit(c)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = c;
        }
        return true;
    }
}
