using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Energy;

/// <summary>Represents a validated Energy Identification Code (EIC).</summary>
public readonly struct EicValue
{
    /// <summary>Gets the normalized EIC code string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="EicValue"/> struct.</summary>
    /// <param name="value">The code string.</param>
    public EicValue(string value) => Value = value;
}

/// <summary>Provides validation for EIC codes.</summary>
public static class Eic
{
    /// <summary>Attempts to validate the supplied input as an EIC code.</summary>
    /// <param name="input">Candidate code to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EicValue> result)
    {
        Span<char> buffer = stackalloc char[16];
        if (!Normalize(input, buffer, out int len) || len != 16)
        {
            result = new ValidationResult<EicValue>(false, default, ValidationError.Length);
            return false;
        }
        if (buffer[0] < '0' || buffer[0] > '9' || buffer[1] < '0' || buffer[1] > '9' ||
            "XYZTWAV".IndexOf(buffer[2]) < 0 || buffer[15] == '-' || ComputeCheckCharacter(buffer[..15]) != buffer[15])
        {
            result = new ValidationResult<EicValue>(false, default, ValidationError.Checksum);
            return false;
        }
        string value = new string(buffer);
        result = new ValidationResult<EicValue>(true, new EicValue(value), ValidationError.None);
        return true;
    }

    /// <summary>Returns false because unrestricted EIC generation is unavailable into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation was attempted; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Returns false because unrestricted EIC generation is unavailable using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 16) return false;
        // ENTSO-E restricts algorithm use for allocation to authorised issuing offices.
        // Arbitrary synthetic generation is therefore deliberately unavailable.
        return false;
    }

    private static char ComputeCheckCharacter(ReadOnlySpan<char> body)
    {
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-";
        int sum = 0;
        for (int i = 0; i < body.Length; i++) sum += alphabet.IndexOf(body[i]) * (16 - i);
        return alphabet[(37 - sum % 37) % 37];
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int written)
    {
        written = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            char u = char.ToUpperInvariant(ch);
            if (!((u >= '0' && u <= '9') || (u >= 'A' && u <= 'Z') || u == '-')) { written = 0; return false; }
            if (written >= dest.Length) { written = 0; return false; }
            dest[written++] = u;
        }
        return true;
    }
}

