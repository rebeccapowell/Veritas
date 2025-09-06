using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Education;

/// <summary>Represents a validated International Standard Music Number.</summary>
public readonly struct IsmnValue
{
    public string Value { get; }
    public IsmnValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for ISMN identifiers.</summary>
public static class Ismn
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IsmnValue> result)
    {
        Span<char> buf = stackalloc char[13];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char up = char.ToUpperInvariant(ch);
            if (len >= buf.Length) { result = new ValidationResult<IsmnValue>(false, default, ValidationError.Length); return true; }
            buf[len++] = up;
        }
        if (len == 10)
        {
            if (buf[0] != 'M') { result = new ValidationResult<IsmnValue>(false, default, ValidationError.Format); return true; }
            for (int i = 1; i < 10; i++)
                if (buf[i] < '0' || buf[i] > '9') { result = new ValidationResult<IsmnValue>(false, default, ValidationError.Charset); return true; }
            int sum = 3 * 3; // 'M' treated as 3
            for (int i = 1; i < 9; i++)
            {
                int d = buf[i] - '0';
                sum += d * ((i % 2 == 1) ? 1 : 3);
            }
            int check = (10 - (sum % 10)) % 10;
            if (buf[9] - '0' != check) { result = new ValidationResult<IsmnValue>(false, default, ValidationError.Checksum); return true; }
            result = new ValidationResult<IsmnValue>(true, new IsmnValue(new string(buf[..10])), ValidationError.None);
            return true;
        }
        else if (len == 13)
        {
            for (int i = 0; i < 13; i++)
                if (buf[i] < '0' || buf[i] > '9') { result = new ValidationResult<IsmnValue>(false, default, ValidationError.Charset); return true; }
            if (!Gs1.Validate(buf)) { result = new ValidationResult<IsmnValue>(false, default, ValidationError.Checksum); return true; }
            result = new ValidationResult<IsmnValue>(true, new IsmnValue(new string(buf)), ValidationError.None);
            return true;
        }
        else
        {
            result = new ValidationResult<IsmnValue>(false, default, ValidationError.Length);
            return true;
        }
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
        destination[0] = '9';
        destination[1] = '7';
        destination[2] = '9';
        destination[3] = '0';
        for (int i = 4; i < 12; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[12] = (char)('0' + Gs1.ComputeCheckDigit(destination[..12]));
        written = 13;
        return true;
    }
}
