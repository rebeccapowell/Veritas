using System;
using Veritas;

namespace Veritas.Tax.US;

public readonly struct SsnValue { public string Value { get; } public SsnValue(string v) => Value = v; }

public static class Ssn
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<SsnValue> result)
    {
        Span<char> digits = stackalloc char[9];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == '-' || ch == ' ') continue;
            if (ch < '0' || ch > '9') { result = new ValidationResult<SsnValue>(false, default, ValidationError.Charset); return true; }
            if (len >= 9) { result = new ValidationResult<SsnValue>(false, default, ValidationError.Length); return true; }
            digits[len++] = ch;
        }
        if (len != 9) { result = new ValidationResult<SsnValue>(false, default, ValidationError.Length); return true; }
        if (digits[0] == '0' && digits[1] == '0' && digits[2] == '0') { result = new ValidationResult<SsnValue>(false, default, ValidationError.CountryRule); return true; }
        int area = (digits[0]-'0')*100 + (digits[1]-'0')*10 + (digits[2]-'0');
        if (area == 0 || area == 666 || area >= 900) { result = new ValidationResult<SsnValue>(false, default, ValidationError.CountryRule); return true; }
        if (digits[3] == '0' && digits[4] == '0') { result = new ValidationResult<SsnValue>(false, default, ValidationError.CountryRule); return true; }
        if (digits[5] == '0' && digits[6] == '0' && digits[7] == '0' && digits[8] == '0') { result = new ValidationResult<SsnValue>(false, default, ValidationError.CountryRule); return true; }
        result = new ValidationResult<SsnValue>(true, new SsnValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 9)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int area;
        do
        {
            area = rng.Next(1, 900); // 001-899
        } while (area == 666);
        int group = rng.Next(1, 100); // 01-99
        int serial = rng.Next(1, 10000); // 0001-9999
        destination[0] = (char)('0' + area / 100);
        destination[1] = (char)('0' + (area / 10) % 10);
        destination[2] = (char)('0' + area % 10);
        destination[3] = (char)('0' + group / 10);
        destination[4] = (char)('0' + group % 10);
        destination[5] = (char)('0' + serial / 1000);
        destination[6] = (char)('0' + (serial / 100) % 10);
        destination[7] = (char)('0' + (serial / 10) % 10);
        destination[8] = (char)('0' + serial % 10);
        written = 9;
        return true;
    }
}
