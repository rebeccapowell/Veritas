using System;
using Veritas;

namespace Veritas.Logistics;

public readonly struct AwbValue { public string Value { get; } public AwbValue(string v) => Value = v; }

public static class Awb
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<AwbValue> result)
    {
        Span<char> digits = stackalloc char[11];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if ((uint)(ch - '0') > 9)
            {
                result = new ValidationResult<AwbValue>(false, default, ValidationError.Charset);
                return true;
            }
            if (len >= 11)
            {
                result = new ValidationResult<AwbValue>(false, default, ValidationError.Length);
                return true;
            }
            digits[len++] = ch;
        }
        if (len != 11)
        {
            result = new ValidationResult<AwbValue>(false, default, ValidationError.Length);
            return true;
        }
        int serial = 0;
        for (int i = 3; i < 10; i++)
            serial = serial * 10 + (digits[i] - '0');
        int check = serial % 7;
        if (digits[10] - '0' != check)
        {
            result = new ValidationResult<AwbValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<AwbValue>(true, new AwbValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 11)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 10; i++)
            destination[i] = (char)('0' + rng.Next(10));
        int serial = 0;
        for (int i = 3; i < 10; i++)
            serial = serial * 10 + (destination[i] - '0');
        destination[10] = (char)('0' + (serial % 7));
        written = 11;
        return true;
    }
}

