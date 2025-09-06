using System;
using Veritas;

namespace Veritas.Telecom;

public readonly struct OuiValue { public string Value { get; } public OuiValue(string v) => Value = v; }

public static class Oui
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<OuiValue> result)
    {
        Span<char> buf = stackalloc char[6];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == '-' || ch == ':' || ch == '.') continue;
            char c = char.ToUpperInvariant(ch);
            if (!Uri.IsHexDigit(c))
            {
                result = new ValidationResult<OuiValue>(false, default, ValidationError.Charset);
                return true;
            }
            if (len >= 6)
            {
                result = new ValidationResult<OuiValue>(false, default, ValidationError.Length);
                return true;
            }
            buf[len++] = c;
        }
        if (len != 6)
        {
            result = new ValidationResult<OuiValue>(false, default, ValidationError.Length);
            return true;
        }
        result = new ValidationResult<OuiValue>(true, new OuiValue(new string(buf)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 6) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 6; i++)
        {
            int v = rng.Next(16);
            destination[i] = (char)(v < 10 ? '0' + v : 'A' + v - 10);
        }
        written = 6;
        return true;
    }
}
