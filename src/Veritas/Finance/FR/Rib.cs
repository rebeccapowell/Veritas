using System;
using Veritas;

namespace Veritas.Finance.FR;

/// <summary>French RIB (Relevé d'Identité Bancaire) including the two-digit key.</summary>
public readonly struct RibValue
{
    /// <summary>The normalized RIB string.</summary>
    public string Value { get; }
    public RibValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for French RIB numbers.</summary>
public static class Rib
{
    /// <summary>Validates the supplied RIB.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<RibValue> result)
    {
        Span<char> buffer = stackalloc char[23];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (len >= buffer.Length) { result = new ValidationResult<RibValue>(false, default, ValidationError.Length); return false; }
            buffer[len++] = char.IsLower(ch) ? (char)(ch - 32) : ch;
        }
        if (len != 23)
        {
            result = new ValidationResult<RibValue>(false, default, ValidationError.Length);
            return false;
        }
        for (int i = 0; i < 5; i++)
            if (buffer[i] < '0' || buffer[i] > '9') { result = new ValidationResult<RibValue>(false, default, ValidationError.Charset); return false; }
        for (int i = 5; i < 10; i++)
            if (buffer[i] < '0' || buffer[i] > '9') { result = new ValidationResult<RibValue>(false, default, ValidationError.Charset); return false; }
        for (int i = 10; i < 21; i++)
        {
            char c = buffer[i];
            if (c >= 'a' && c <= 'z') c = (char)(c - 32);
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z'))) { result = new ValidationResult<RibValue>(false, default, ValidationError.Charset); return false; }
            buffer[i] = c;
        }
        if (buffer[21] < '0' || buffer[21] > '9' || buffer[22] < '0' || buffer[22] > '9')
        {
            result = new ValidationResult<RibValue>(false, default, ValidationError.Charset);
            return false;
        }
        int bank = ParseInt(buffer[..5]);
        int branch = ParseInt(buffer.Slice(5, 5));
        long account = ParseAccount(buffer.Slice(10, 11));
        int expected = 97 - (int)((89L * bank + 15L * branch + 3L * account) % 97L);
        int key = (buffer[21] - '0') * 10 + (buffer[22] - '0');
        if (expected == 0) expected = 97;
        if (expected != key)
        {
            result = new ValidationResult<RibValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<RibValue>(true, new RibValue(new string(buffer)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a valid RIB into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a valid RIB using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 23)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 5; i++) destination[i] = (char)('0' + rng.Next(10));
        for (int i = 5; i < 10; i++) destination[i] = (char)('0' + rng.Next(10));
        for (int i = 10; i < 21; i++) destination[i] = (char)('0' + rng.Next(10));
        int bank = ParseInt(destination[..5]);
        int branch = ParseInt(destination.Slice(5, 5));
        long account = ParseAccount(destination.Slice(10, 11));
        int key = 97 - (int)((89L * bank + 15L * branch + 3L * account) % 97L);
        if (key == 0) key = 97;
        destination[21] = (char)('0' + key / 10);
        destination[22] = (char)('0' + key % 10);
        written = 23;
        return true;
    }

    private static int ParseInt(ReadOnlySpan<char> digits)
    {
        int n = 0;
        foreach (var ch in digits) n = n * 10 + (ch - '0');
        return n;
    }

    private static long ParseAccount(ReadOnlySpan<char> span)
    {
        long n = 0;
        foreach (var ch in span)
        {
            int v;
            if (ch >= '0' && ch <= '9') v = ch - '0';
            else v = ch switch
            {
                'A' => 1,
                'B' => 2,
                'C' => 3,
                'D' => 4,
                'E' => 5,
                'F' => 6,
                'G' => 7,
                'H' => 8,
                'I' => 9,
                'J' => 1,
                'K' => 2,
                'L' => 3,
                'M' => 4,
                'N' => 5,
                'O' => 6,
                'P' => 7,
                'Q' => 8,
                'R' => 9,
                'S' => 2,
                'T' => 3,
                'U' => 4,
                'V' => 5,
                'W' => 6,
                'X' => 7,
                'Y' => 8,
                'Z' => 9,
                _ => 0
            };
            n = n * 10 + v;
        }
        return n;
    }
}

