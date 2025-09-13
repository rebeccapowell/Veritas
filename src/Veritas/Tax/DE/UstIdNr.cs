using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Tax.DE;

public readonly struct UstIdNrValue
{
    public string Value { get; }
    public UstIdNrValue(string value) => Value = value;
}

public static class UstIdNr
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<UstIdNrValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<UstIdNrValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9)
        {
            result = new ValidationResult<UstIdNrValue>(false, default, ValidationError.Length);
            return false;
        }
        if (digits[0] == '0')
        {
            result = new ValidationResult<UstIdNrValue>(false, default, ValidationError.Format);
            return false;
        }
        char check = Iso7064.ComputeCheckDigitMod11_10(digits[..8]);
        if (digits[8] != check)
        {
            result = new ValidationResult<UstIdNrValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<UstIdNrValue>(true, new UstIdNrValue(new string(digits)), ValidationError.None);
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
        digits[0] = (char)('1' + rng.Next(9));
        for (int i = 1; i < 8; i++)
            digits[i] = (char)('0' + rng.Next(10));
        digits[8] = Iso7064.ComputeCheckDigitMod11_10(digits[..8]);
        written = 9;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        bool skippedPrefix = false;
        for (int i = 0; i < input.Length; i++)
        {
            char ch = input[i];
            if (ch == ' ' || ch == '.' || ch == '-' || ch == '/') continue;
            if (!skippedPrefix && (ch == 'D' || ch == 'd'))
            {
                if (i + 1 < input.Length && (input[i + 1] == 'E' || input[i + 1] == 'e'))
                {
                    skippedPrefix = true;
                    i++; // skip E
                    continue;
                }
            }
            if (ch < '0' || ch > '9') return false;
            if (len >= dest.Length) return false;
            dest[len++] = ch;
        }
        return true;
    }
}
