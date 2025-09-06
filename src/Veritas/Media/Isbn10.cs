using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Media;

public readonly struct Isbn10Value { public string Value { get; } public Isbn10Value(string v) => Value = v; }

public static class Isbn10
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<Isbn10Value> result)
    {
        Span<char> buf = stackalloc char[10];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == '-' || ch == ' ') continue;
            char u = char.ToUpperInvariant(ch);
            if ((u < '0' || u > '9') && !(u == 'X' && len == 9)) { result = new ValidationResult<Isbn10Value>(false, default, ValidationError.Charset); return true; }
            if (len >= 10) { result = new ValidationResult<Isbn10Value>(false, default, ValidationError.Length); return true; }
            buf[len++] = u;
        }
        if (len != 10) { result = new ValidationResult<Isbn10Value>(false, default, ValidationError.Length); return true; }
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (10 - i) * (buf[i] - '0');
        }
        int check = 11 - (sum % 11);
        char expected = check == 10 ? 'X' : check == 11 ? '0' : (char)('0' + check);
        if (buf[9] != expected) { result = new ValidationResult<Isbn10Value>(false, default, ValidationError.Checksum); return true; }
        result = new ValidationResult<Isbn10Value>(true, new Isbn10Value(new string(buf)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 10)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 9; i++)
            destination[i] = (char)('0' + rng.Next(10));
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (10 - i) * (destination[i] - '0');
        int check = 11 - (sum % 11);
        destination[9] = check == 10 ? 'X' : check == 11 ? '0' : (char)('0' + check);
        written = 10;
        return true;
    }
}
