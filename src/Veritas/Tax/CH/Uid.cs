using System;
using Veritas;

namespace Veritas.Tax.CH;

public readonly struct UidValue
{
    public string Value { get; }
    public UidValue(string value) => Value = value;
}

/// <summary>
/// Swiss UID (CHE) enterprise identifier with mod-10 (EAN) check digit.
/// </summary>
public static class Uid
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<UidValue> result)
    {
        Span<char> digits = stackalloc char[9];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<UidValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 9)
        {
            result = new ValidationResult<UidValue>(false, default, ValidationError.Length);
            return false;
        }
        int check = ComputeCheckDigit(digits[..8]);
        if (digits[8] - '0' != check)
        {
            result = new ValidationResult<UidValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<UidValue>(true, new UidValue("CHE" + new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        // require space for "CHE" + 9 digits => 12 characters
        if (destination.Length < 12) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = 'C';
        destination[1] = 'H';
        destination[2] = 'E';
        Span<char> digits = destination.Slice(3, 9);
        for (int i = 0; i < 8; i++)
            digits[i] = (char)('0' + rng.Next(10));
        digits[8] = (char)('0' + ComputeCheckDigit(digits[..8]));
        written = 12;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        for (int i = 0; i < digits.Length; i++)
        {
            int d = digits[digits.Length - 1 - i] - '0';
            sum += d * (i % 2 == 0 ? 3 : 1);
        }
        int r = 10 - (sum % 10);
        return r == 10 ? 0 : r;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        bool prefixSeen = false;
        int i = 0;
        while (i < input.Length && char.IsWhiteSpace(input[i])) i++;
        if (i + 3 <= input.Length && (input[i] == 'C' || input[i] == 'c') && (input[i + 1] == 'H' || input[i + 1] == 'h') && (input[i + 2] == 'E' || input[i + 2] == 'e'))
        {
            prefixSeen = true;
            i += 3;
        }
        for (; i < input.Length; i++)
        {
            char ch = input[i];
            if (ch == ' ' || ch == '.' || ch == '-') continue;
            if (ch == 'M' || ch == 'm') break; // stop at VAT suffix
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return prefixSeen;
    }
}

