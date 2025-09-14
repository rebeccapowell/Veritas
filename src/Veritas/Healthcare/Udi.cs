using System;

namespace Veritas.Healthcare;

/// <summary>Represents a validated Unique Device Identifier (UDI).</summary>
public readonly struct UdiValue
{
    /// <summary>Gets the normalized UDI string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="UdiValue"/> struct.</summary>
    /// <param name="value">Normalized UDI.</param>
    public UdiValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Unique Device Identifiers.</summary>
public static class Udi
{
    /// <summary>Attempts to validate the supplied UDI.</summary>
    /// <param name="input">Candidate UDI.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<UdiValue> result)
    {
        if (input.IsEmpty)
        {
            result = new ValidationResult<UdiValue>(false, default, ValidationError.Length);
            return false;
        }

        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ')
                continue;
            buf[len++] = ch;
        }

        if (len == 0)
        {
            result = new ValidationResult<UdiValue>(false, default, ValidationError.Length);
            return false;
        }

        if (buf[0] == '(')
        {
            // GS1 style: require (01) followed by 14 digits
            if (len < 18 || buf[1] != '0' || buf[2] != '1' || buf[3] != ')')
            {
                result = new ValidationResult<UdiValue>(false, default, ValidationError.Format);
                return false;
            }
            for (int i = 4; i < 18; i++)
            {
                if (buf[i] < '0' || buf[i] > '9')
                {
                    result = new ValidationResult<UdiValue>(false, default, ValidationError.Charset);
                    return false;
                }
            }
        }
        else if (buf[0] == '+' || buf[0] == '=')
        {
            // HIBCC or ICCBBA style: alphanumeric after prefix
            for (int i = 1; i < len; i++)
            {
                char u = char.ToUpperInvariant(buf[i]);
                if (!(u >= 'A' && u <= 'Z') && !(u >= '0' && u <= '9'))
                {
                    result = new ValidationResult<UdiValue>(false, default, ValidationError.Charset);
                    return false;
                }
                buf[i] = u;
            }
        }
        else
        {
            result = new ValidationResult<UdiValue>(false, default, ValidationError.Format);
            return false;
        }

        var normalized = new string(buf[..len]);
        result = new ValidationResult<UdiValue>(true, new UdiValue(normalized), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random UDI into the provided buffer.</summary>
    /// <param name="destination">Buffer that receives the generated UDI.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random UDI using the supplied options.</summary>
    /// <param name="options">Options controlling generation.</param>
    /// <param name="destination">Buffer that receives the generated UDI.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        // Generate simple GS1-style UDI: (01) + 14 digits + (21) + 6 alphanumerics
        const int required = 28;
        if (destination.Length < required)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = '(';
        destination[1] = '0';
        destination[2] = '1';
        destination[3] = ')';
        for (int i = 0; i < 14; i++)
            destination[4 + i] = (char)('0' + rng.Next(10));
        destination[18] = '(';
        destination[19] = '2';
        destination[20] = '1';
        destination[21] = ')';
        for (int i = 0; i < 6; i++)
        {
            int v = rng.Next(36);
            destination[22 + i] = v < 10 ? (char)('0' + v) : (char)('A' + (v - 10));
        }
        written = required;
        return true;
    }
}

