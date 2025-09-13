using System;
using Veritas;

namespace Veritas.Logistics;

/// <summary>GS1 Global Individual Asset Identifier (GIAI).</summary>
public readonly struct GiaiValue
{
    /// <summary>The normalized GIAI string.</summary>
    public string Value { get; }
    public GiaiValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for GIAI codes.</summary>
public static class Giai
{
    /// <summary>Validates the supplied GIAI.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<GiaiValue> result)
    {
        Span<char> buffer = stackalloc char[30];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char c = ch;
            if (c >= 'a' && c <= 'z') c = (char)(c - 32);
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z')) || len >= 30)
            {
                result = new ValidationResult<GiaiValue>(false, default, ValidationError.Charset);
                return false;
            }
            buffer[len++] = c;
        }
        if (len == 0 || len > 30)
        {
            result = new ValidationResult<GiaiValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<GiaiValue>(true, new GiaiValue(new string(buffer[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Generates a random GIAI into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a random GIAI using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        int length = 12;
        if (destination.Length < length)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < length; i++)
        {
            int v = rng.Next(36);
            destination[i] = (char)(v < 10 ? '0' + v : 'A' + v - 10);
        }
        written = length;
        return true;
    }
}

