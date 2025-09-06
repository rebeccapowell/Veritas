using System;
using Veritas;

namespace Veritas.Finance;

public readonly struct ClabeValue
{
    public string Value { get; }
    public ClabeValue(string value) => Value = value;
}

public static class Clabe
{
    private static readonly int[] Weights = {3,7,1};

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ClabeValue> result)
    {
        Span<char> digits = stackalloc char[18];
        if (!Normalize(input, digits, out int len) || len != 18)
        {
            result = new ValidationResult<ClabeValue>(false, default, ValidationError.Length);
            return true;
        }
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            int d = digits[i] - '0';
            sum += (d * Weights[i % 3]) % 10;
        }
        int check = (10 - (sum % 10)) % 10;
        if (digits[17] != (char)('0' + check))
        {
            result = new ValidationResult<ClabeValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(digits);
        result = new ValidationResult<ClabeValue>(true, new ClabeValue(value), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 18) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..18];
        for (int i = 0; i < 17; i++)
            digits[i] = (char)('0' + rng.Next(10));
        int sum = 0;
        for (int i = 0; i < 17; i++)
            sum += ((digits[i] - '0') * Weights[i % 3]) % 10;
        digits[17] = (char)('0' + ((10 - (sum % 10)) % 10));
        written = 18;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            if (ch < '0' || ch > '9') { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}
