using Veritas.Checksums;
using Veritas.Algorithms;

namespace Veritas.Identity.Luxembourg;

/// <summary>Provides validation and generation for Luxembourg National ID numbers.</summary>
public static class NationalId
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NationalIdValue> result)
    {
        Span<char> digits = stackalloc char[13];
        if (!Normalize(input, digits, out int len) || len != 13)
        {
            result = new ValidationResult<NationalIdValue>(false, default, ValidationError.Length);
            return true;
        }
        byte luhn = (byte)Luhn.ComputeCheckDigit(digits[..11]);
        if (digits[11] != (char)('0' + luhn))
        {
            result = new ValidationResult<NationalIdValue>(false, default, ValidationError.Checksum);
            return true;
        }
        byte ver = Verhoeff.Compute(digits[..11]);
        if (digits[12] != (char)('0' + ver))
        {
            result = new ValidationResult<NationalIdValue>(false, default, ValidationError.Checksum);
            return true;
        }
        var value = new string(digits);
        result = new ValidationResult<NationalIdValue>(true, new NationalIdValue(value), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 13) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 11; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[11] = (char)('0' + Luhn.ComputeCheckDigit(destination[..11]));
        destination[12] = (char)('0' + Verhoeff.Compute(destination[..11]));
        written = 13;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = ch;
        }
        return true;
    }
}
