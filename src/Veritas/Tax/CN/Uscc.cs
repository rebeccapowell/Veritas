using System;
using Veritas;

namespace Veritas.Tax.CN;

public readonly struct UsccValue
{
    public string Value { get; }
    public UsccValue(string value) => Value = value;
}

public static class Uscc
{
    private const string Alphabet = "0123456789ABCDEFGHJKLMNPQRTUWXY";
    private static readonly int[] Weights = {1,3,9,27,19,26,16,17,20,29,25,13,8,24,10,30,28};

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<UsccValue> result)
    {
        Span<char> chars = stackalloc char[18];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<UsccValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 18)
        {
            result = new ValidationResult<UsccValue>(false, default, ValidationError.Length);
            return true;
        }
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            int v = Alphabet.IndexOf(chars[i]);
            if (v < 0) { result = new ValidationResult<UsccValue>(false, default, ValidationError.Charset); return true; }
            sum += v * Weights[i];
        }
        int check = (31 - (sum % 31)) % 31;
        char expected = Alphabet[check];
        if (chars[17] != expected)
        {
            result = new ValidationResult<UsccValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<UsccValue>(true, new UsccValue(new string(chars)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 18) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> chars = destination[..18];
        for (int i = 0; i < 17; i++)
        {
            chars[i] = Alphabet[rng.Next(Alphabet.Length)];
        }
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            int v = Alphabet.IndexOf(chars[i]);
            sum += v * Weights[i];
        }
        chars[17] = Alphabet[(31 - (sum % 31)) % 31];
        written = 18;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            char c = char.ToUpperInvariant(ch);
            if (Alphabet.IndexOf(c) < 0) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = c;
        }
        return true;
    }
}
