using System;
using Veritas;

namespace Veritas.Identity.Mexico;

public readonly struct CurpValue
{
    public string Value { get; }
    public CurpValue(string value) => Value = value;
}

/// <summary>
/// Mexico Clave Única de Registro de Población (CURP) (18 alphanumeric, mod-10 check digit).
/// </summary>
public static class Curp
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CurpValue> result)
    {
        Span<char> chars = stackalloc char[18];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<CurpValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 18)
        {
            result = new ValidationResult<CurpValue>(false, default, ValidationError.Length);
            return true;
        }
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            int v = Alphabet.IndexOf(chars[i]);
            if (v < 0) { result = new ValidationResult<CurpValue>(false, default, ValidationError.Charset); return true; }
            sum += v * (18 - i);
        }
        int check = (10 - (sum % 10)) % 10;
        if (chars[17] - '0' != check)
        {
            result = new ValidationResult<CurpValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<CurpValue>(true, new CurpValue(new string(chars)), ValidationError.None);
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
            chars[i] = Alphabet[rng.Next(Alphabet.Length)];
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            int v = Alphabet.IndexOf(chars[i]);
            sum += v * (18 - i);
        }
        chars[17] = (char)('0' + ((10 - (sum % 10)) % 10));
        written = 18;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char c = char.ToUpperInvariant(ch);
            if (!char.IsLetterOrDigit(c) && c != 'Ñ') { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = c;
        }
        return true;
    }
}
