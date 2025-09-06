using System;
using Veritas;

namespace Veritas.Media;

public readonly struct IssnValue { public string Value { get; } public IssnValue(string v) => Value = v; }

public static class Issn
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IssnValue> result)
    {
        Span<char> buf = stackalloc char[8];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == '-' || ch == ' ') continue;
            char u = char.ToUpperInvariant(ch);
            if ((u < '0' || u > '9') && !(u == 'X' && len == 7)) { result = new ValidationResult<IssnValue>(false, default, ValidationError.Charset); return true; }
            if (len >= 8) { result = new ValidationResult<IssnValue>(false, default, ValidationError.Length); return true; }
            buf[len++] = u;
        }
        if (len != 8) { result = new ValidationResult<IssnValue>(false, default, ValidationError.Length); return true; }
        int sum = 0;
        for (int i = 0; i < 7; i++) sum += (8 - i) * (buf[i] - '0');
        int check = 11 - (sum % 11);
        char expected = check == 10 ? 'X' : check == 11 ? '0' : (char)('0' + check);
        if (buf[7] != expected) { result = new ValidationResult<IssnValue>(false, default, ValidationError.Checksum); return true; }
        result = new ValidationResult<IssnValue>(true, new IssnValue(new string(buf)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 8)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 7; i++)
            destination[i] = (char)('0' + rng.Next(10));
        int sum = 0;
        for (int i = 0; i < 7; i++) sum += (8 - i) * (destination[i] - '0');
        int check = 11 - (sum % 11);
        destination[7] = check == 10 ? 'X' : check == 11 ? '0' : (char)('0' + check);
        written = 8;
        return true;
    }
}
