using System;
using Veritas;

namespace Veritas.Tax.IT;

public readonly struct CodiceFiscaleValue
{
    public string Value { get; }
    public CodiceFiscaleValue(string value) => Value = value;
}

/// <summary>
/// Italy Codice Fiscale (16 alphanumeric, position table check character).
/// </summary>
public static class CodiceFiscale
{
    private const string EvenMap = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly int[] OddMap = new int[36]
    {
        1,0,5,7,9,13,15,17,19,21,
        1,0,5,7,9,13,15,17,19,21,
        2,4,18,20,11,3,6,8,12,14,
        16,10,22,25,24,23
    };

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CodiceFiscaleValue> result)
    {
        Span<char> chars = stackalloc char[16];
        if (!Normalize(input, chars, out int len))
        {
            result = new ValidationResult<CodiceFiscaleValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 16)
        {
            result = new ValidationResult<CodiceFiscaleValue>(false, default, ValidationError.Length);
            return true;
        }
        int sum = 0;
        for (int i = 0; i < 15; i++)
        {
            char c = chars[i];
            int idx = EvenMap.IndexOf(c);
            if (idx < 0) { result = new ValidationResult<CodiceFiscaleValue>(false, default, ValidationError.Charset); return true; }
            if ((i & 1) == 0) sum += OddMap[idx]; else sum += idx;
        }
        char check = (char)('A' + (sum % 26));
        if (chars[15] != check)
        {
            result = new ValidationResult<CodiceFiscaleValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<CodiceFiscaleValue>(true, new CodiceFiscaleValue(new string(chars)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 16) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> chars = destination[..16];
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string months = "ABCDEHLMPRST"; // official month codes
        for (int i = 0; i < 6; i++) chars[i] = letters[rng.Next(letters.Length)];
        for (int i = 6; i < 8; i++) chars[i] = (char)('0' + rng.Next(10));
        chars[8] = months[rng.Next(months.Length)];
        int day = rng.Next(1, 31);
        chars[9] = (char)('0' + day / 10);
        chars[10] = (char)('0' + day % 10);
        chars[11] = letters[rng.Next(letters.Length)];
        for (int i = 12; i < 15; i++) chars[i] = (char)('0' + rng.Next(10));
        int sum = 0;
        for (int i = 0; i < 15; i++)
        {
            char c = chars[i];
            int idx = EvenMap.IndexOf(c);
            sum += ((i & 1) == 0) ? OddMap[idx] : idx;
        }
        chars[15] = (char)('A' + (sum % 26));
        written = 16;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char c = char.ToUpperInvariant(ch);
            if (!char.IsLetterOrDigit(c)) { len = 0; return false; }
            if (len >= dest.Length) { len = 0; return false; }
            dest[len++] = c;
        }
        return true;
    }
}
