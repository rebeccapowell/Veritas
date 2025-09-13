using System;
using Veritas;

namespace Veritas.Tax.PT;

public readonly struct NifValue
{
    public string Value { get; }
    public NifValue(string value) => Value = value;
}

/// <summary>
/// Portugal Número de Identificação Fiscal (NIF) (9 digits, mod-11 checksum).
/// </summary>
public static class Nif
{
    private static readonly int[] AllowedFirst = new[] { 1, 2, 3, 5, 6, 8, 9 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NifValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<NifValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9)
        {
            result = new ValidationResult<NifValue>(false, default, ValidationError.Length);
            return false;
        }
        int first = digits[0] - '0';
        bool okFirst = false;
        foreach (var a in AllowedFirst)
            if (first == a) { okFirst = true; break; }
        if (!okFirst)
        {
            result = new ValidationResult<NifValue>(false, default, ValidationError.Format);
            return false;
        }
        int sum = 0;
        for (int i = 0; i < 8; i++)
            sum += (digits[i] - '0') * (9 - i);
        int check = 11 - (sum % 11);
        if (check >= 10) check = 0;
        if (digits[8] - '0' != check)
        {
            result = new ValidationResult<NifValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<NifValue>(true, new NifValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 9) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..9];
        int first = AllowedFirst[rng.Next(AllowedFirst.Length)];
        digits[0] = (char)('0' + first);
        for (int i = 1; i < 8; i++)
            digits[i] = (char)('0' + rng.Next(10));
        int sum = 0;
        for (int i = 0; i < 8; i++)
            sum += (digits[i] - '0') * (9 - i);
        int check = 11 - (sum % 11);
        if (check >= 10) check = 0;
        digits[8] = (char)('0' + check);
        written = 9;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '.') continue;
            if (!char.IsDigit(ch) || len >= dest.Length)
            {
                len = 0;
                return false;
            }
            dest[len++] = ch;
        }
        return true;
    }
}

