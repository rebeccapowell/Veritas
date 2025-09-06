using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Finance;

public readonly struct LeiValue
{
    public string Value { get; }
    public LeiValue(string value) => Value = value;
}

public static class Lei
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<LeiValue> result)
    {
        Span<char> chars = stackalloc char[20];
        if (!Normalize(input, chars, out int len) || len != 20)
        {
            result = new ValidationResult<LeiValue>(false, default, ValidationError.Length);
            return true;
        }
        Span<char> digits = stackalloc char[40];
        int idx = 0;
        for (int i = 0; i < 20; i++)
            Append(digits, ref idx, chars[i]);
        if (Iso7064.ComputeMod97(digits[..idx]) != 1)
        {
            result = new ValidationResult<LeiValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(chars);
        result = new ValidationResult<LeiValue>(true, new LeiValue(value), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 20) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> chars = destination[..20];
        for (int i = 0; i < 18; i++)
        {
            int v = rng.Next(36);
            chars[i] = v < 10 ? (char)('0' + v) : (char)('A' + v - 10);
        }
        Span<char> digits = stackalloc char[40];
        int idx = 0;
        for (int i = 0; i < 18; i++)
            Append(digits, ref idx, chars[i]);
        digits[idx++] = '0';
        digits[idx++] = '0';
        int cd = Iso7064.ComputeCheckDigitsMod97_10(digits[..idx]);
        chars[18] = (char)('0' + cd / 10);
        chars[19] = (char)('0' + cd % 10);
        written = 20;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            char u = char.ToUpperInvariant(ch);
            if (!(char.IsDigit(u) || (u >= 'A' && u <= 'Z'))) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = u;
        }
        return true;
    }

    private static void Append(Span<char> dest, ref int idx, char ch)
    {
        if (ch >= 'A' && ch <= 'Z')
        {
            int v = ch - 'A' + 10;
            dest[idx++] = (char)('0' + v / 10);
            dest[idx++] = (char)('0' + v % 10);
        }
        else
        {
            dest[idx++] = ch;
        }
    }
}
