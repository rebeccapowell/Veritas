using System;
using Veritas;

namespace Veritas.Tax.MX;

public readonly struct RfcValue
{
    public string Value { get; }
    public RfcValue(string value) => Value = value;
}

/// <summary>
/// Mexico Registro Federal de Contribuyentes (RFC) (12-13 alphanumeric, mod-11 check character).
/// </summary>
public static class Rfc
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMN&OPQRSTUVWXYZÑ";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<RfcValue> result)
    {
        Span<char> chars = stackalloc char[13];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<RfcValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 12 && len != 13)
        {
            result = new ValidationResult<RfcValue>(false, default, ValidationError.Length);
            return false;
        }
        int totalLen = len;
        int sum = 0;
        for (int i = 0; i < totalLen - 1; i++)
        {
            int v = Alphabet.IndexOf(chars[i]);
            if (v < 0) { result = new ValidationResult<RfcValue>(false, default, ValidationError.Charset); return false; }
            sum += v * (totalLen - i);
        }
        int checkVal = 11 - (sum % 11);
        char expected = checkVal == 11 ? '0' : checkVal == 10 ? 'A' : (char)('0' + checkVal);
        if (chars[totalLen - 1] != expected)
        {
            result = new ValidationResult<RfcValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<RfcValue>(true, new RfcValue(new string(chars[..totalLen])), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 13) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> chars = destination[..13];
        for (int i = 0; i < 12; i++)
            chars[i] = Alphabet[rng.Next(36)];
        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            int v = Alphabet.IndexOf(chars[i]);
            sum += v * (13 - i);
        }
        int checkVal = 11 - (sum % 11);
        chars[12] = checkVal == 11 ? '0' : checkVal == 10 ? 'A' : (char)('0' + checkVal);
        written = 13;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == '-' || ch == ' ') continue;
            char c = char.ToUpperInvariant(ch);
            if (!char.IsLetterOrDigit(c) && c != '&' && c != 'Ñ') { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = c;
        }
        return true;
    }
}
