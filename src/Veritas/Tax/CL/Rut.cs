using System;
using Veritas;

namespace Veritas.Tax.CL;

public readonly struct RutValue
{
    public string Value { get; }
    public RutValue(string value) => Value = value;
}

/// <summary>
/// Chilean Rol Único Tributario (RUT) identifier with mod-11 check digit.
/// </summary>
public static class Rut
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<RutValue> result)
    {
        Span<char> buffer = stackalloc char[9];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<RutValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len < 2 || len > 9)
        {
            result = new ValidationResult<RutValue>(false, default, ValidationError.Length);
            return true;
        }
        char expected = ComputeCheckChar(buffer[..(len - 1)]);
        if (buffer[len - 1] != expected)
        {
            result = new ValidationResult<RutValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(buffer[..len]);
        result = new ValidationResult<RutValue>(true, new RutValue(value), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 9) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..9];
        for (int i = 0; i < 8; i++)
            digits[i] = (char)('0' + rng.Next(10));
        digits[8] = ComputeCheckChar(digits[..8]);
        written = 9;
        return true;
    }

    private static char ComputeCheckChar(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        int weight = 2;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            sum += (digits[i] - '0') * weight;
            weight = weight == 7 ? 2 : weight + 1;
        }
        int r = 11 - (sum % 11);
        if (r == 11) return '0';
        if (r == 10) return 'K';
        return (char)('0' + r);
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == '.' || ch == '-' || ch == ' ') continue;
            char up = char.ToUpperInvariant(ch);
            if (!char.IsDigit(ch) && up != 'K') { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = up;
        }
        return true;
    }
}

