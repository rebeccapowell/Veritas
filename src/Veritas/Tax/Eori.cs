using System;
using Veritas;

namespace Veritas.Tax;

/// <summary>Represents a validated Economic Operator Registration and Identification (EORI) number.</summary>
/// <example>Eori.TryValidate("DE123456789012345", out var value);</example>
public readonly struct EoriValue
{
    /// <summary>The normalized EORI string.</summary>
    public string Value { get; }
    public EoriValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for EORI numbers.</summary>
public static class Eori
{
    /// <summary>Validates the supplied EORI string.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EoriValue> result)
    {
        Span<char> buffer = stackalloc char[17];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char c = ch;
            if (len < 2)
            {
                if (c >= 'a' && c <= 'z') c = (char)(c - 32);
                if (c < 'A' || c > 'Z') { result = new ValidationResult<EoriValue>(false, default, ValidationError.Charset); return false; }
                buffer[len++] = c;
            }
            else
            {
                if (c >= 'a' && c <= 'z') c = (char)(c - 32);
                if (!((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))) { result = new ValidationResult<EoriValue>(false, default, ValidationError.Charset); return false; }
                if (len >= buffer.Length) { result = new ValidationResult<EoriValue>(false, default, ValidationError.Length); return false; }
                buffer[len++] = c;
            }
        }
        if (len < 3 || len > 17)
        {
            result = new ValidationResult<EoriValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<EoriValue>(true, new EoriValue(new string(buffer[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Generates a valid EORI into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a valid EORI using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        var cc = _countryCodes[rng.Next(_countryCodes.Length)];
        int tailLen = 1 + rng.Next(15); // 1..15
        int total = 2 + tailLen;
        if (destination.Length < total)
        {
            written = 0;
            return false;
        }
        destination[0] = cc[0];
        destination[1] = cc[1];
        for (int i = 0; i < tailLen; i++)
            destination[2 + i] = (char)('0' + rng.Next(10));
        written = total;
        return true;
    }

    private static readonly string[] _countryCodes = new[]
    {
        "AT","BE","BG","CY","CZ","DE","DK","EE","ES","FI","FR","GR","HR","HU","IE","IT","LT","LU","LV","MT","NL","PL","PT","RO","SE","SI","SK","GB"
    };
}
