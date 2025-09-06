using System;
using Veritas;

namespace Veritas.Finance;

public readonly struct SedolValue
{
    public string Value { get; }
    public SedolValue(string value) => Value = value;
}

public static class Sedol
{
    private static readonly int[] Weights = {1,3,1,7,3,9,1};

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<SedolValue> result)
    {
        Span<char> chars = stackalloc char[7];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<SedolValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 7)
        {
            result = new ValidationResult<SedolValue>(false, default, ValidationError.Length);
            return true;
        }
        int sum = 0;
        for (int i = 0; i < 6; i++)
        {
            int v = ValueOf(chars[i]);
            if (v < 0) { result = new ValidationResult<SedolValue>(false, default, ValidationError.Charset); return true; }
            sum += v * Weights[i];
        }
        int check = (10 - (sum % 10)) % 10;
        if (chars[6] - '0' != check)
        {
            result = new ValidationResult<SedolValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<SedolValue>(true, new SedolValue(new string(chars)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 7) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> chars = destination[..7];
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < 6; i++)
            chars[i] = alphabet[rng.Next(alphabet.Length)];
        int sum = 0;
        for (int i = 0; i < 6; i++)
            sum += ValueOf(chars[i]) * Weights[i];
        chars[6] = (char)('0' + ((10 - (sum % 10)) % 10));
        written = 7;
        return true;
    }

    private static int ValueOf(char c)
    {
        if (char.IsDigit(c)) return c - '0';
        if (c >= 'A' && c <= 'Z') return c - 'A' + 10;
        return -1;
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
