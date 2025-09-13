using System;

namespace Veritas.IntellectualProperty;

/// <summary>Validation and generation for copyright registration numbers.</summary>
/// <example>
/// <code language="csharp">
/// Span&lt;char&gt; dst = stackalloc char[12];
/// CopyrightRegistrationNumber.TryGenerate(default, dst, out _);
/// </code>
/// </example>
public static class CopyrightRegistrationNumber
{
    private static readonly string[] _prefixes = { "TX", "VA", "SR" };

    /// <summary>Validates a copyright registration number.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CopyrightRegistrationNumberValue> result)
    {
        Span<char> buf = stackalloc char[12];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (len >= 12) { result = new ValidationResult<CopyrightRegistrationNumberValue>(false, default, ValidationError.Length); return false; }
            char c = ch;
            if (c >= 'a' && c <= 'z') c = (char)(c - 32);
            if (len < 2)
            {
                if (c < 'A' || c > 'Z') { result = new ValidationResult<CopyrightRegistrationNumberValue>(false, default, ValidationError.Charset); return false; }
            }
            else
            {
                if (c < '0' || c > '9') { result = new ValidationResult<CopyrightRegistrationNumberValue>(false, default, ValidationError.Charset); return false; }
            }
            buf[len++] = c;
        }
        if (len != 12)
        {
            result = new ValidationResult<CopyrightRegistrationNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<CopyrightRegistrationNumberValue>(true, new CopyrightRegistrationNumberValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a copyright registration number into the destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a copyright registration number using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 12) { written = 0; return false; }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        var prefix = _prefixes[rng.Next(_prefixes.Length)];
        destination[0] = prefix[0];
        destination[1] = prefix[1];
        int year = rng.Next(1978, DateTime.UtcNow.Year + 1);
        destination[2] = (char)('0' + (year / 1000 % 10));
        destination[3] = (char)('0' + (year / 100 % 10));
        destination[4] = (char)('0' + (year / 10 % 10));
        destination[5] = (char)('0' + (year % 10));
        int serial = rng.Next(0, 1_000_000);
        for (int i = 0; i < 6; i++)
        {
            destination[11 - i] = (char)('0' + (serial % 10));
            serial /= 10;
        }
        written = 12;
        return true;
    }
}
