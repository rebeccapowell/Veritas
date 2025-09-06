using System;
using Veritas;

namespace Veritas.Healthcare;

public readonly struct NhsNumberValue { public string Value { get; } public NhsNumberValue(string v) => Value = v; }

public static class NhsNumber
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NhsNumberValue> result)
    {
        Span<char> digits = stackalloc char[10];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<NhsNumberValue>(false, default, ValidationError.Charset); return true; }
            if (len >= 10) { result = new ValidationResult<NhsNumberValue>(false, default, ValidationError.Length); return true; }
            digits[len++] = ch;
        }
        if (len != 10) { result = new ValidationResult<NhsNumberValue>(false, default, ValidationError.Length); return true; }
        int sum = 0;
        for (int i = 0; i < 9; i++) sum += (10 - i) * (digits[i] - '0');
        int check = 11 - (sum % 11);
        if (check == 11) check = 0;
        if (check == 10 || check != digits[9] - '0') { result = new ValidationResult<NhsNumberValue>(false, default, ValidationError.Checksum); return true; }
        result = new ValidationResult<NhsNumberValue>(true, new NhsNumberValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 10)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination;
        while (true)
        {
            for (int i = 0; i < 9; i++)
                digits[i] = (char)('0' + rng.Next(10));
            int sum = 0;
            for (int i = 0; i < 9; i++) sum += (10 - i) * (digits[i] - '0');
            int check = 11 - (sum % 11);
            if (check == 11) check = 0;
            if (check == 10) continue;
            digits[9] = (char)('0' + check);
            written = 10;
            return true;
        }
    }
}
