using System;
using Veritas;

namespace Veritas.Tax.IN;

public readonly struct GstinValue
{
    public string Value { get; }
    public GstinValue(string value) => Value = value;
}

public static class Gstin
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly int[] Weights = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<GstinValue> result)
    {
        Span<char> chars = stackalloc char[15];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<GstinValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 15)
        {
            result = new ValidationResult<GstinValue>(false, default, ValidationError.Length);
            return false;
        }
        int sum = 0;
        for (int i = 0; i < 14; i++)
        {
            int v = Alphabet.IndexOf(chars[i]);
            if (v < 0) { result = new ValidationResult<GstinValue>(false, default, ValidationError.Charset); return false; }
            sum += v * Weights[i];
        }
        int check = (36 - (sum % 36)) % 36;
        char expected = Alphabet[check];
        if (chars[14] != expected)
        {
            result = new ValidationResult<GstinValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<GstinValue>(true, new GstinValue(new string(chars)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 15) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> chars = destination[..15];
        for (int i = 0; i < 14; i++)
        {
            int idx = rng.Next(Alphabet.Length);
            chars[i] = Alphabet[idx];
        }
        int sum = 0;
        for (int i = 0; i < 14; i++)
        {
            int v = Alphabet.IndexOf(chars[i]);
            sum += v * Weights[i];
        }
        chars[14] = Alphabet[(36 - (sum % 36)) % 36];
        written = 15;
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
