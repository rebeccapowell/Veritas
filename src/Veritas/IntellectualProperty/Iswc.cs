using System;

namespace Veritas.IntellectualProperty;

/// <summary>Validation and generation for International Standard Musical Work Codes (ISWC).</summary>
/// <example>
/// <code language="csharp">
/// Span&lt;char&gt; dst = stackalloc char[11];
/// Iswc.TryGenerate(default, dst, out _);
/// </code>
/// </example>
public static class Iswc
{
    /// <summary>Validates the supplied ISWC.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IswcValue> result)
    {
        Span<char> buf = stackalloc char[11];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-' || ch == '.') continue;
            if (len >= 11) { result = new ValidationResult<IswcValue>(false, default, ValidationError.Length); return false; }
            char c = ch;
            if (len == 0)
            {
                if (c >= 'a' && c <= 'z') c = (char)(c - 32);
                if (c != 'T') { result = new ValidationResult<IswcValue>(false, default, ValidationError.Format); return false; }
            }
            else
            {
                if (c < '0' || c > '9') { result = new ValidationResult<IswcValue>(false, default, ValidationError.Charset); return false; }
            }
            buf[len++] = c;
        }
        if (len != 11)
        {
            result = new ValidationResult<IswcValue>(false, default, ValidationError.Length);
            return false;
        }
        int sum = 1;
        for (int i = 1; i <= 9; i++) sum += (buf[i] - '0') * i;
        int check = (10 - (sum % 10)) % 10;
        if (buf[10] - '0' != check)
        {
            result = new ValidationResult<IswcValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<IswcValue>(true, new IswcValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a valid ISWC into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a valid ISWC using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 11) { written = 0; return false; }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = 'T';
        for (int i = 1; i <= 9; i++) destination[i] = (char)('0' + rng.Next(10));
        int sum = 1;
        for (int i = 1; i <= 9; i++) sum += (destination[i] - '0') * i;
        destination[10] = (char)('0' + ((10 - (sum % 10)) % 10));
        written = 11;
        return true;
    }
}
