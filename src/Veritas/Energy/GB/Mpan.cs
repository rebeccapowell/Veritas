using System;
using Veritas;

namespace Veritas.Energy.GB;

public readonly struct MpanValue
{
    public string Value { get; }
    public MpanValue(string value) => Value = value;
}

public static class Mpan
{
    private static readonly int[] Weights = new[] { 3, 7, 1, 3, 7, 1, 3, 7, 1, 3, 7, 1 };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MpanValue> result)
    {
        Span<char> digits = stackalloc char[13];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<MpanValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 13)
        {
            result = new ValidationResult<MpanValue>(false, default, ValidationError.Length);
            return true;
        }
        int sum = 0;
        for (int i = 0; i < 12; i++)
            sum += (digits[i] - '0') * Weights[i];
        int r = sum % 11;
        int check = r == 10 ? 0 : r;
        if (digits[12] - '0' != check)
        {
            result = new ValidationResult<MpanValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(digits);
        result = new ValidationResult<MpanValue>(true, new MpanValue(value), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 13)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 12; i++)
            destination[i] = (char)('0' + rng.Next(10));
        int sum = 0;
        for (int i = 0; i < 12; i++)
            sum += (destination[i] - '0') * Weights[i];
        int r = sum % 11;
        destination[12] = (char)('0' + (r == 10 ? 0 : r));
        written = 13;
        return true;
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

