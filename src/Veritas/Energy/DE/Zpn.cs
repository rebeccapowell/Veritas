using System;
using Veritas;

namespace Veritas.Energy.DE;

public readonly struct ZpnValue
{
    public string Value { get; }
    public ZpnValue(string value) => Value = value;
}

public static class Zpn
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ZpnValue> result)
    {
        Span<char> chars = stackalloc char[33];
        if (!Normalize(input, chars, out int len) || len != 33)
        {
            result = new ValidationResult<ZpnValue>(false, default, ValidationError.Length);
            return true;
        }
        if (chars[0] != 'D' || chars[1] != 'E')
        {
            result = new ValidationResult<ZpnValue>(false, default, ValidationError.Format);
            return true;
        }
        string value = new string(chars);
        result = new ValidationResult<ZpnValue>(true, new ZpnValue(value), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 33) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> chars = destination[..33];
        chars[0] = 'D';
        chars[1] = 'E';
        for (int i = 2; i < 33; i++)
        {
            int v = rng.Next(36);
            chars[i] = v < 10 ? (char)('0' + v) : (char)('A' + v - 10);
        }
        written = 33;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            char u = char.ToUpperInvariant(ch);
            if (!(char.IsDigit(u) || (u >= 'A' && u <= 'Z')))
            {
                len = 0;
                return false;
            }
            if (len >= dest.Length)
            {
                len = 0;
                return false;
            }
            dest[len++] = u;
        }
        return true;
    }
}
