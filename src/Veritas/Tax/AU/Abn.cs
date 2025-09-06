using System;
using Veritas;

namespace Veritas.Tax.AU;

public readonly struct AbnValue
{
    public string Value { get; }
    public AbnValue(string value) => Value = value;
}

public static class Abn
{
    private static readonly int[] Weights = {10,1,3,5,7,9,11,13,15,17,19};

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<AbnValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<AbnValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 11)
        {
            result = new ValidationResult<AbnValue>(false, default, ValidationError.Length);
            return true;
        }
        int sum = (digits[0] - '1') * Weights[0];
        for (int i = 1; i < 11; i++)
            sum += (digits[i] - '0') * Weights[i];
        if (sum % 89 != 0)
        {
            result = new ValidationResult<AbnValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<AbnValue>(true, new AbnValue(new string(digits)), ValidationError.None);
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
        for (int i = 0; i < 10; i++)
            digits[i] = (char)('0' + rng.Next(10));
        for (int d = 0; d < 10; d++)
        {
            digits[10] = (char)('0' + d);
            int sum = (digits[0] - '1') * Weights[0];
            for (int i = 1; i < 11; i++) sum += (digits[i] - '0') * Weights[i];
            if (sum % 89 == 0)
            {
                written = 11;
                return true;
            }
        }
        return false;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}
