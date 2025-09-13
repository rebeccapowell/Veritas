using Veritas.Checksums;

namespace Veritas.Healthcare.Snomed;

/// <summary>Provides validation and generation for SNOMED CT Concept IDs.</summary>
public static class SctId
{
    private const int MinLength = 6;
    private const int MaxLength = 18;

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<SctIdValue> result)
    {
        Span<char> digits = stackalloc char[MaxLength];
        if (!Normalize(input, digits, out int len) || len < MinLength || len > MaxLength)
        {
            result = new ValidationResult<SctIdValue>(false, default, ValidationError.Length);
            return false;
        }
        if (!Verhoeff.Validate(digits[..len]))
        {
            result = new ValidationResult<SctIdValue>(false, default, ValidationError.Checksum);
            return false;
        }
        var value = new string(digits[..len]);
        result = new ValidationResult<SctIdValue>(true, new SctIdValue(value), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(int length, Span<char> destination, out int written)
        => TryGenerate(length, default, destination, out written);

    public static bool TryGenerate(int length, in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (length < MinLength || length > MaxLength) return false;
        if (destination.Length < length) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < length - 1; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[length - 1] = (char)('0' + Verhoeff.Compute(destination[..(length - 1)]));
        written = length;
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
