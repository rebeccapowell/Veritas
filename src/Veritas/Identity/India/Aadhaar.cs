using Veritas.Checksums;

namespace Veritas.Identity.India;

/// <summary>Provides validation and generation for Indian Aadhaar numbers.</summary>
public static class Aadhaar
{
    /// <summary>Attempts to validate the supplied input as an Aadhaar number.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<AadhaarValue> result)
    {
        Span<char> digits = stackalloc char[12];
        if (!Normalize(input, digits, out int len) || len != 12)
        {
            result = new ValidationResult<AadhaarValue>(false, default, ValidationError.Length);
            return true;
        }
        bool allSame = true;
        for (int i = 1; i < 12; i++)
            if (digits[i] != digits[0]) { allSame = false; break; }
        if (allSame)
        {
            result = new ValidationResult<AadhaarValue>(false, default, ValidationError.Format);
            return true;
        }
        if (!Verhoeff.Validate(digits))
        {
            result = new ValidationResult<AadhaarValue>(false, default, ValidationError.Checksum);
            return true;
        }
        var value = new string(digits);
        result = new ValidationResult<AadhaarValue>(true, new AadhaarValue(value), ValidationError.None);
        return true;
    }

    /// <summary>Generates a test Aadhaar number.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 12) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 11; i++)
            destination[i] = (char)('0' + rng.Next(10));
        bool allSame = true;
        for (int i = 1; i < 11; i++)
            if (destination[i] != destination[0]) { allSame = false; break; }
        if (allSame)
            destination[10] = destination[10] == '0' ? '1' : '0';
        destination[11] = (char)('0' + Verhoeff.Compute(destination[..11]));
        written = 12;
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
