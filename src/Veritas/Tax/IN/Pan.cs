using System;
using Veritas;

namespace Veritas.Tax.IN;

public readonly struct PanValue
{
    public string Value { get; }
    public PanValue(string value) => Value = value;
}

public static class Pan
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PanValue> result)
    {
        Span<char> chars = stackalloc char[10];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<PanValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 10)
        {
            result = new ValidationResult<PanValue>(false, default, ValidationError.Length);
            return true;
        }
        // pattern: 5 letters, 4 digits, 1 letter
        for (int i = 0; i < 5; i++) if (!char.IsLetter(chars[i])) { result = new ValidationResult<PanValue>(false, default, ValidationError.Charset); return true; }
        for (int i = 5; i < 9; i++) if (!char.IsDigit(chars[i])) { result = new ValidationResult<PanValue>(false, default, ValidationError.Charset); return true; }
        if (!char.IsLetter(chars[9])) { result = new ValidationResult<PanValue>(false, default, ValidationError.Charset); return true; }
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int v = Alphabet.IndexOf(chars[i]);
            if (v < 0) { result = new ValidationResult<PanValue>(false, default, ValidationError.Charset); return true; }
            sum += v * (i + 1);
        }
        int check = sum % 36;
        char expected = Alphabet[check];
        if (chars[9] != expected)
        {
            result = new ValidationResult<PanValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<PanValue>(true, new PanValue(new string(chars)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 10) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> chars = destination[..10];
        while (true)
        {
            for (int i = 0; i < 5; i++) chars[i] = (char)('A' + rng.Next(26));
            for (int i = 5; i < 9; i++) chars[i] = (char)('0' + rng.Next(10));
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                int v = Alphabet.IndexOf(chars[i]);
                sum += v * (i + 1);
            }
            int check = sum % 36;
            if (check >= 10)
            {
                chars[9] = Alphabet[check];
                written = 10;
                return true;
            }
            // checksum resulted in a digit; regenerate
        }
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
