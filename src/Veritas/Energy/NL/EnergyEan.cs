using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Energy.NL;

public readonly struct EnergyEanValue
{
    public string Value { get; }
    public EnergyEanValue(string value) => Value = value;
}

public static class EnergyEan
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EnergyEanValue> result)
    {
        Span<char> digits = stackalloc char[18];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<EnergyEanValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 18)
        {
            result = new ValidationResult<EnergyEanValue>(false, default, ValidationError.Length);
            return true;
        }
        if (!Gs1.Validate(digits))
        {
            result = new ValidationResult<EnergyEanValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(digits);
        result = new ValidationResult<EnergyEanValue>(true, new EnergyEanValue(value), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 18)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = stackalloc char[17];
        for (int i = 0; i < 17; i++)
            digits[i] = (char)('0' + rng.Next(10));
        int check = Gs1.ComputeCheckDigit(digits);
        digits.CopyTo(destination);
        destination[17] = (char)('0' + check);
        written = 18;
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
