using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Tax.DE;

public readonly struct IdNrValue
{
    public string Value { get; }
    public IdNrValue(string value) => Value = value;
}

public static class IdNr
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IdNrValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<IdNrValue>(false, default, ValidationError.Format);
            return false;
        }
        if (len != 11)
        {
            result = new ValidationResult<IdNrValue>(false, default, ValidationError.Length);
            return false;
        }
        if (digits[0] == '0')
        {
            result = new ValidationResult<IdNrValue>(false, default, ValidationError.Format);
            return false;
        }
        Span<int> counts = stackalloc int[10];
        for (int i = 0; i < 10; i++)
            counts[digits[i] - '0']++;
        int repeatGroups = 0; int repeatCount = 0;
        for (int i = 0; i < 10; i++)
        {
            if (counts[i] > 1)
            {
                repeatGroups++;
                repeatCount = counts[i];
            }
        }
        if (!(repeatGroups == 1 && (repeatCount == 2 || repeatCount == 3)))
        {
            result = new ValidationResult<IdNrValue>(false, default, ValidationError.Format);
            return false;
        }
        if (!Iso7064.ValidateMod11_10(digits))
        {
            result = new ValidationResult<IdNrValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<IdNrValue>(true, new IdNrValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 11) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..11];
        int repeated = rng.Next(0, 10);
        int repeatCount = rng.Next(2, 4); // 2 or 3
        Span<int> repeatPos = stackalloc int[3];
        for (int i = 0; i < repeatCount; i++)
        {
            int pos;
            do
            {
                pos = rng.Next(0, 10);
            } while ((repeated == 0 && pos == 0) || Contains(repeatPos[..i], pos));
            repeatPos[i] = pos;
        }
        Span<bool> used = stackalloc bool[10];
        used[repeated] = true;
        for (int i = 0; i < 10; i++)
        {
            if (Contains(repeatPos[..repeatCount], i))
            {
                digits[i] = (char)('0' + repeated);
            }
            else
            {
                int d;
                do
                {
                    d = rng.Next(0, 10);
                } while (used[d] || (i == 0 && d == 0));
                digits[i] = (char)('0' + d);
                used[d] = true;
            }
        }
        digits[10] = Iso7064.ComputeCheckDigitMod11_10(digits[..10]);
        written = 11;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '.' || ch == '-' || ch == '/') continue;
            if (ch < '0' || ch > '9') return false;
            if (len >= dest.Length) return false;
            dest[len++] = ch;
        }
        return true;
    }

    private static bool Contains(ReadOnlySpan<int> span, int value)
    {
        for (int i = 0; i < span.Length; i++)
            if (span[i] == value) return true;
        return false;
    }
}
