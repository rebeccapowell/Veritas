using System;
using Veritas;

namespace Veritas.Tax.SG;

public readonly struct UenValue
{
    public string Value { get; }
    public UenValue(string value) => Value = value;
}

/// <summary>
/// Singapore Unique Entity Number (UEN) (9-10 alphanumeric, weighted mod-11 check).
/// </summary>
public static class Uen
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<UenValue> result)
    {
        Span<char> chars = stackalloc char[10];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<UenValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9 && len != 10)
        {
            result = new ValidationResult<UenValue>(false, default, ValidationError.Length);
            return false;
        }
        int sum = 0;
        for (int i = 0; i < len - 1; i++)
        {
            int v = Alphabet.IndexOf(chars[i]);
            if (v < 0) { result = new ValidationResult<UenValue>(false, default, ValidationError.Charset); return false; }
            sum += v * (len - i);
        }
        int checkVal = (11 - (sum % 11)) % 11;
        char expected = checkVal == 10 ? 'A' : (char)('0' + checkVal);
        if (chars[len - 1] != expected)
        {
            result = new ValidationResult<UenValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<UenValue>(true, new UenValue(new string(chars[..len])), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 10) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int len = rng.Next(0, 2) == 0 ? 9 : 10;
        Span<char> chars = destination[..len];
        for (int i = 0; i < len - 1; i++)
            chars[i] = Alphabet[rng.Next(Alphabet.Length)];
        int sum = 0;
        for (int i = 0; i < len - 1; i++)
        {
            int v = Alphabet.IndexOf(chars[i]);
            sum += v * (len - i);
        }
        int checkVal = (11 - (sum % 11)) % 11;
        chars[len - 1] = checkVal == 10 ? 'A' : (char)('0' + checkVal);
        written = len;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char c = char.ToUpperInvariant(ch);
            if (!char.IsLetterOrDigit(c)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = c;
        }
        return true;
    }
}
